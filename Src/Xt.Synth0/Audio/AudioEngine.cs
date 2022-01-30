using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	unsafe class AudioEngine : IDisposable
	{
		const float OverloadLimit = 0.9f;
		const float InfoDurationSeconds = 0.5f;
		const float CpuUsageUpdateIntervalSeconds = 0.2f;
		const float CpuUsageSamplingPeriodSeconds = 0.5f;

		static XtSample DepthToSample(int size) => size switch
		{
			16 => XtSample.Int16,
			24 => XtSample.Int24,
			32 => XtSample.Int32,
			_ => throw new InvalidOperationException()
		};

		internal static AudioEngine Create(IntPtr mainWindow, SettingsModel settings,
			SynthModel synth, Action<string> log, Action<Action> dispatchToUI)
		{
			XtAudio.SetOnError(msg => log(msg));
			var platform = XtAudio.Init(nameof(Synth0), mainWindow);
			try
			{
				return Create(platform, settings, synth, dispatchToUI);
			}
			catch
			{
				platform.Dispose();
				throw;
			}
		}

		static AudioEngine Create(XtPlatform platform, SettingsModel settings,
			SynthModel synth, Action<Action> dispatchToUI)
		{
			var asio = platform.GetService(XtSystem.ASIO);
			var wasapi = platform.GetService(XtSystem.WASAPI);
			return new AudioEngine(platform, settings, synth, dispatchToUI,
				asio.GetDefaultDeviceId(true),
				wasapi.GetDefaultDeviceId(true),
				GetDevices(asio), GetDevices(wasapi));
		}

		static IReadOnlyList<DeviceModel> GetDevices(XtService service)
		{
			var result = new List<DeviceModel>();
			using var list = service.OpenDeviceList(XtEnumFlags.Output);
			for (int d = 0; d < list.GetCount(); d++)
			{
				var id = list.GetId(d);
				result.Add(new DeviceModel(id, list.GetName(id)));
			}
			return new ReadOnlyCollection<DeviceModel>(result);
		}

		float* _buffer;
		int _cpuUsageIndex;
		double[] _cpuUsageFactors;
		int[] _cpuUsageFrameCounts;
		int _cpuUsageTotalFrameCount;
		readonly int[] _automationValues;

		readonly Action _stopStream;
		readonly XtPlatform _platform;
		readonly Action<Action> _dispatchToUI;

		readonly SynthModel _synth;
		readonly SettingsModel _settings;
		readonly SynthModel _localSynth = new();
		readonly SynthModel _originalSynth = new();

		IntPtr _nativeDSP;
		StreamModel _streamUI;
		IAudioStream _audioStream;
		SeqModel.Native* _nativeSeq;
		Native.SeqState* _nativeState;
		SynthModel.Native* _nativeSynth;
		SynthModel.Native.VoiceBinding* _nativeBinding;

		long _clipPosition = -1;
		long _cpuUsagePosition = -1;
		long _overloadPosition = -1;
		long _voiceInfoPosition = -1;
		long _exhaustedPosition = -1;
		long _bufferInfoPosition = -1;
		readonly long[] _gcPositions = new long[3];
		readonly bool[] _gcCollecteds = new bool[3];
		readonly Stopwatch _stopwatch = new Stopwatch();

		public string AsioDefaultDeviceId { get; }
		public string WasapiDefaultDeviceId { get; }
		public IReadOnlyList<DeviceModel> AsioDevices { get; }
		public IReadOnlyList<DeviceModel> WasapiDevices { get; }

		AudioEngine(
			XtPlatform platform,
			SettingsModel settings,
			SynthModel synth,
			Action<Action> dispatchToUI,
			string asioDefaultDeviceId,
			string wasapiDefaultDeviceId,
			IReadOnlyList<DeviceModel> asioDevices,
			IReadOnlyList<DeviceModel> wasapiDevices)
		{
			GCNotification.Register(this);

			AsioDevices = asioDevices;
			WasapiDevices = wasapiDevices;
			AsioDefaultDeviceId = asioDefaultDeviceId;
			WasapiDefaultDeviceId = wasapiDefaultDeviceId;

			_synth = synth;
			_settings = settings;
			_platform = platform;
			_stopStream = StopStream;
			_dispatchToUI = dispatchToUI;
			_automationValues = new int[_originalSynth.Params.Count];

			_nativeDSP = Native.XtsSeqDSPCreate();
			_nativeState = Native.XtsSeqStateCreate();
			_nativeSynth = Native.XtsSynthModelCreate();
			_nativeBinding = Native.XtsVoiceBindingCreate();
			_synth.BindVoice(_nativeSynth, _nativeBinding);
		}

		internal void OnGCNotification(int generation)
		=> _gcCollecteds[generation] = true;

		public void Dispose()
		{
			Reset();
			_platform.Dispose();
			Native.XtsSeqDSPDestroy(_nativeDSP);
			Native.XtsSeqStateDestroy(_nativeState);
			Native.XtsVoiceBindingDestroy(_nativeBinding);
			Native.XtsSynthModelDestroy(_nativeSynth);
		}

		internal void Reset()
		{
			StopStream();
			_audioStream?.Dispose();
			_audioStream = null;
		}

		internal void Stop(bool pause)
		{
			if (pause && _streamUI?.IsRunning == true)
				PauseStream();
			else
				StopStream();
		}

		internal void Start(SeqModel seq, StreamModel stream)
		{
			if (_streamUI != null && _streamUI != stream)
				throw new InvalidOperationException();
			_streamUI = stream;
			if (_streamUI.IsPaused)
				ResumeStream();
			else
				StartStream(seq);
		}

		void PauseStream()
		{
			try
			{
				_audioStream?.Stop();
				_originalSynth.CopyTo(_synth);
				_streamUI.State = StreamState.Paused;
			}
			catch
			{
				StopStream();
				throw;
			}
		}

		void ResumeStream()
		{
			try
			{
				AutomationQueue.Clear();
				_synth.CopyTo(_localSynth);
				_synth.CopyTo(_originalSynth);
				_synth.ToNative(_nativeBinding);
				_streamUI.State = StreamState.Running;
				_audioStream.Start();
			}
			catch
			{
				StopStream();
				throw;
			}
		}

		void StopStream()
		{
			if (_streamUI == null) return;

			PauseStream();

			Native.XtsSeqModelDestroy(_nativeSeq);
			_nativeSeq = null;
			_stopwatch.Reset();

			Marshal.FreeHGlobal(new IntPtr(_buffer));
			_buffer = null;
			_clipPosition = -1;
			_overloadPosition = -1;
			_cpuUsagePosition = -1;
			_voiceInfoPosition = -1;
			_exhaustedPosition = -1;
			_bufferInfoPosition = -1;

			_cpuUsageIndex = 0;
			_cpuUsageFactors = null;
			_cpuUsageFrameCounts = null;
			_cpuUsageTotalFrameCount = 0;

			_streamUI.Voices = 0;
			_streamUI.CpuUsage = 0.0;
			_streamUI.LatencyMs = 0.0;
			_streamUI.IsClipping = false;
			_streamUI.IsExhausted = false;
			_streamUI.IsOverloaded = false;
			_streamUI.GC0Collected = false;
			_streamUI.GC1Collected = false;
			_streamUI.GC2Collected = false;
			_streamUI.BufferSizeFrames = 0;
			_streamUI.State = StreamState.Stopped;
			_streamUI = null;

			for (int i = 0; i < _gcPositions.Length; i++)
			{
				_gcPositions[i] = -1;
				_gcCollecteds[i] = false;
			}
		}

		void StartStream(SeqModel seq)
		{
			try
			{
				var format = GetFormat();
				var bufferSize = _settings.BufferSize.ToInt();
				var streamParams = new XtStreamParams(true, OnXtBuffer, null, OnXtRunning);
				var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
				if (_audioStream == null)
					if (_settings.WriteToDisk)
						_audioStream = new DiskStream(this, in format, bufferSize, _settings.OutputPath);
					else
						_audioStream = OpenDeviceStream(in deviceParams);
				_cpuUsageIndex = 0;
				_cpuUsageTotalFrameCount = 0;
				_cpuUsageFactors = new double[format.mix.rate];
				_cpuUsageFrameCounts = new int[format.mix.rate];
				_buffer = (float*)Marshal.AllocHGlobal(_audioStream.GetMaxBufferFrames() * sizeof(float) * 2);
				UpdateStreamInfo(0, format.mix.rate, 0, false, false, 0);
				_nativeSeq = Native.XtsSeqModelCreate();
				seq.ToNative(_nativeSeq);
				Native.XtsSeqDSPInit(_nativeDSP, _nativeSeq, _nativeSynth, _nativeBinding);
				ResumeStream();
			}
			catch
			{
				StopStream();
				throw;
			}
		}

		void ResetWarnings(int rate, long streamPosition)
		{
			float infoFrames = InfoDurationSeconds * rate;
			if (streamPosition > _clipPosition + infoFrames)
				_streamUI.IsClipping = false;
			if (streamPosition > _gcPositions[0] + infoFrames)
				_streamUI.GC0Collected = false;
			if (streamPosition > _gcPositions[1] + infoFrames)
				_streamUI.GC1Collected = false;
			if (streamPosition > _gcPositions[2] + infoFrames)
				_streamUI.GC2Collected = false;
			if (streamPosition > _overloadPosition + infoFrames)
				_streamUI.IsOverloaded = false;
			if (streamPosition > _exhaustedPosition + infoFrames)
				_streamUI.IsExhausted = false;
		}

		void UpdateStreamInfo(int frames, int rate, int voices, bool clip, bool exhausted, long streamPosition)
		{
			float bufferSeconds = frames / (float)rate;
			var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
			if (clip)
			{
				_streamUI.IsClipping = true;
				_clipPosition = streamPosition;
			}
			if (exhausted)
			{
				_streamUI.IsExhausted = true;
				_exhaustedPosition = streamPosition;
			}
			if (_gcCollecteds[0])
			{
				_gcCollecteds[0] = false;
				_streamUI.GC0Collected = true;
				_gcPositions[0] = streamPosition;
			}
			if (_gcCollecteds[1])
			{
				_gcCollecteds[1] = false;
				_streamUI.GC1Collected = true;
				_gcPositions[1] = streamPosition;
			}
			if (_gcCollecteds[2])
			{
				_gcCollecteds[2] = false;
				_streamUI.GC2Collected = true;
				_gcPositions[2] = streamPosition;
			}
			if (processedSeconds > bufferSeconds * OverloadLimit)
			{
				_overloadPosition = streamPosition;
				_streamUI.IsOverloaded = true;
			}
			if (_bufferInfoPosition == -1 || streamPosition >=
				_bufferInfoPosition + rate * InfoDurationSeconds)
			{
				_bufferInfoPosition = streamPosition;
				_streamUI.LatencyMs = _audioStream.GetLatencyMs();
				_streamUI.BufferSizeFrames = _audioStream.GetMaxBufferFrames();
			}
			if (streamPosition >= _voiceInfoPosition + rate * InfoDurationSeconds)
			{
				_voiceInfoPosition = streamPosition;
				_streamUI.Voices = voices;
			}
		}

		void UpdateCpuUsage(int frames, int rate, long streamPosition)
		{
			float bufferSeconds = frames / (float)rate;
			var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
			int cpuUsageCountToRemove = 0;
			_cpuUsageTotalFrameCount += frames;
			_cpuUsageFrameCounts[_cpuUsageIndex] = frames;
			_cpuUsageFactors[_cpuUsageIndex] = Math.Min(processedSeconds / bufferSeconds, 1.0);
			while (_cpuUsageTotalFrameCount > CpuUsageSamplingPeriodSeconds * rate)
				_cpuUsageTotalFrameCount -= _cpuUsageFrameCounts[_cpuUsageIndex - cpuUsageCountToRemove++];
			for (int i = 0; i <= _cpuUsageIndex - cpuUsageCountToRemove; i++)
			{
				_cpuUsageFactors[i] = _cpuUsageFactors[i + cpuUsageCountToRemove];
				_cpuUsageFrameCounts[i] = _cpuUsageFrameCounts[i + cpuUsageCountToRemove];
			}
			_cpuUsageIndex -= cpuUsageCountToRemove;
			double cpuUsage = 0.0;
			for (int i = 0; i <= _cpuUsageIndex; i++)
				cpuUsage += _cpuUsageFactors[i] * _cpuUsageFrameCounts[i] / _cpuUsageTotalFrameCount;
			_cpuUsageIndex++;
			if (streamPosition > _cpuUsagePosition + CpuUsageUpdateIntervalSeconds * rate)
			{
				_streamUI.CpuUsage = cpuUsage;
				_cpuUsagePosition = streamPosition;
			}
		}

		void CopyBuffer(in XtBuffer buffer, in XtFormat format)
		{
			switch (format.mix.sample)
			{
				case XtSample.Int16: CopyBuffer16(buffer); break;
				case XtSample.Int24: CopyBuffer24(buffer); break;
				case XtSample.Int32: CopyBuffer32(buffer); break;
				default: throw new InvalidOperationException();
			}
		}

		unsafe void CopyBuffer32(in XtBuffer buffer)
		{
			int* samples = (int*)buffer.output;
			for (int f = 0; f < buffer.frames; f++)
			{
				samples[f * 2] = (int)(_buffer[f * 2] * int.MaxValue);
				samples[f * 2 + 1] = (int)(_buffer[f * 2 + 1] * int.MaxValue);
			}
		}

		unsafe void CopyBuffer16(in XtBuffer buffer)
		{
			short* samples = (short*)buffer.output;
			for (int f = 0; f < buffer.frames; f++)
			{
				samples[f * 2] = (short)(_buffer[f * 2] * short.MaxValue);
				samples[f * 2 + 1] = (short)(_buffer[f * 2 + 1] * short.MaxValue);
			}
		}

		unsafe void CopyBuffer24(in XtBuffer buffer)
		{
			byte* bytes = (byte*)buffer.output;
			for (int f = 0; f < buffer.frames; f++)
			{
				int left = (int)(_buffer[f * 2] * int.MaxValue);
				int right = (int)(_buffer[f * 2 + 1] * int.MaxValue);
				bytes[f * 6 + 0] = (byte)((left & 0x0000FF00) >> 8);
				bytes[f * 6 + 1] = (byte)((left & 0x00FF0000) >> 16);
				bytes[f * 6 + 2] = (byte)((left & 0xFF000000) >> 24);
				bytes[f * 6 + 3] = (byte)((right & 0x0000FF00) >> 8);
				bytes[f * 6 + 4] = (byte)((right & 0x00FF0000) >> 16);
				bytes[f * 6 + 5] = (byte)((right & 0xFF000000) >> 24);
			}
		}

		void BeginAutomation()
		{
			var @params = _localSynth.Params;
			var actions = AutomationQueue.DequeueUI(out var count);
			for (int i = 0; i < count; i++)
				@params[actions[i].Param].Value = @actions[i].Value;
			for (int i = 0; i < @params.Count; i++)
				_automationValues[i] = @params[i].Value;
			_localSynth.ToNative(_nativeBinding);
		}

		void EndAutomation()
		{
			_localSynth.FromNative(_nativeBinding);
			var @params = _localSynth.Params;
			for (int i = 0; i < @params.Count; i++)
				if (@params[i].Value != _automationValues[i])
					AutomationQueue.EnqueueAudio(i, @params[i].Value);
		}

		internal unsafe void OnBuffer(in XtBuffer buffer, in XtFormat format)
		{
			_stopwatch.Restart();
			BeginAutomation();
			int rate = format.mix.rate;
			var state = _nativeState;
			state->rate = rate;
			state->buffer = _buffer;
			state->frames = buffer.frames;
			state->seq = _nativeSeq;
			state->synth = _nativeSynth;
			Native.XtsSeqDSPRender(_nativeDSP, state);
			EndAutomation();
			long pos = _nativeState->pos;
			CopyBuffer(in buffer, in format);
			ResetWarnings(rate, pos);
			_streamUI.CurrentRow = _nativeState->row;
			_stopwatch.Stop();
			UpdateCpuUsage(buffer.frames, rate, pos);
			bool clip = state->clip != 0;
			bool exhausted = state->exhausted != 0;
			UpdateStreamInfo(buffer.frames, rate, state->voices, clip, exhausted, pos);
			if (state->end != 0) _dispatchToUI(_stopStream);
		}

		internal void OnRunning(bool running, ulong error)
		{
			if (!running && error != 0)
				_dispatchToUI(StopStream);
		}

		int OnXtBuffer(XtStream stream, in XtBuffer buffer, object user)
		{
			OnBuffer(buffer, stream.GetFormat());
			return 0;
		}

		void OnXtRunning(XtStream stream, bool running, ulong error, object user)
		{
			OnRunning(running, error);
		}

		internal XtBufferSize? QueryFormatSupport()
		{
			var format = GetFormat();
			using var device = OpenDevice();
			if (!device.SupportsFormat(in format)) return null;
			return device.GetBufferSize(in format);
		}

		XtFormat GetFormat()
		{
			var depth = _settings.BitDepth.ToInt();
			var rate = _settings.SampleRate.ToInt();
			var sample = DepthToSample(depth);
			var mix = new XtMix(rate, sample);
			var channels = new XtChannels(0, 0, 2, 0);
			return new XtFormat(in mix, in channels);
		}

		XtDevice OpenDevice(XtSystem system, string deviceId, string defaultId)
		{
			var service = _platform.GetService(system);
			var id = string.IsNullOrEmpty(deviceId) ? defaultId : deviceId;
			return service.OpenDevice(id);
		}

		XtDevice OpenDevice()
		{
			var system = _settings.UseAsio ? XtSystem.ASIO : XtSystem.WASAPI;
			var selectedId = _settings.UseAsio ? _settings.AsioDeviceId : _settings.WasapiDeviceId;
			var defaultId = _settings.UseAsio ? AsioDefaultDeviceId : WasapiDefaultDeviceId;
			return OpenDevice(system, selectedId, defaultId);
		}

		internal void ShowASIOControlPanel(string deviceId)
		{
			using var device = OpenDevice(XtSystem.ASIO, deviceId, AsioDefaultDeviceId);
			device.ShowControlPanel();
		}

		IAudioStream OpenDeviceStream(in XtDeviceStreamParams deviceParams)
		{
			XtDevice device = OpenDevice();
			try
			{
				var stream = device.OpenStream(in deviceParams, null);
				return new DeviceStream(device, stream);
			}
			catch
			{
				device.Dispose();
				throw;
			}
		}
	}
}