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

		internal static AudioEngine Create(IntPtr mainWindow, SettingsModel settings, StreamModel stream,
			SynthModel synth, Action<string> log, Action asyncStop, Action<Action> dispatchToUI)
		{
			XtAudio.SetOnError(msg => log(msg));
			var platform = XtAudio.Init(nameof(Synth0), mainWindow);
			try
			{
				return Create(platform, settings, stream, synth, asyncStop, dispatchToUI);
			}
			catch
			{
				platform.Dispose();
				throw;
			}
		}

		static AudioEngine Create(XtPlatform platform, SettingsModel settings,
			StreamModel stream, SynthModel synth, Action asyncStop, Action<Action> dispatchToUI)
		{
			var asio = platform.GetService(XtSystem.ASIO);
			var wasapi = platform.GetService(XtSystem.WASAPI);
			return new AudioEngine(platform, settings, stream, synth, asyncStop, dispatchToUI,
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

		readonly Action _asyncStop;
		readonly XtPlatform _platform;
		readonly Action<Action> _dispatchToUI;

		readonly SynthModel _synthModel;
		readonly StreamModel _streamModel;
		readonly SettingsModel _settingsModel;
		readonly SynthModel _originalSynthModel = new();

		IAudioStream _stream;
		IntPtr _nativeSeqDSP;
		Native.SeqState* _nativeSeqState;
		SeqModel.Native* _nativeSeqModel;
		SynthModel.Native* _nativeSynthModel;
		SynthModel.Native.VoiceBinding* _nativeBinding;
		readonly SynthModel _managedSynthModel = new();

		long _clipPosition = -1;
		long _cpuUsagePosition = -1;
		long _overloadPosition = -1;
		long _voiceInfoPosition = -1;
		long _exhaustedPosition = -1;
		long _bufferInfoPosition = -1;
		readonly long[] _gcPositions = new long[3];
		readonly bool[] _gcCollecteds = new bool[3];
		readonly Stopwatch _stopwatch = new Stopwatch();

		float* _buffer;
		int _cpuUsageIndex;
		double[] _cpuUsageFactors;
		int[] _cpuUsageFrameCounts;
		int _cpuUsageTotalFrameCount;
		readonly int[] _automationValues;

		public string AsioDefaultDeviceId { get; }
		public string WasapiDefaultDeviceId { get; }
		public IReadOnlyList<DeviceModel> AsioDevices { get; }
		public IReadOnlyList<DeviceModel> WasapiDevices { get; }

		AudioEngine(
			XtPlatform platform,
			SettingsModel settings,
			StreamModel stream,
			SynthModel synth,
			Action asyncStop,
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

			_platform = platform;
			_synthModel = synth;
			_streamModel = stream;
			_settingsModel = settings;
			_asyncStop = asyncStop;
			_dispatchToUI = dispatchToUI;
			_automationValues = new int[_originalSynthModel.Params.Count];

			_nativeSeqDSP = Native.XtsSeqDSPCreate();
			_nativeSeqState = Native.XtsSeqStateCreate();
			_nativeBinding = Native.XtsVoiceBindingCreate();
			_nativeSynthModel = Native.XtsSynthModelCreate();
			_synthModel.BindVoice(_nativeSynthModel, _nativeBinding);
		}

		internal void OnGCNotification(int generation)
		=> _gcCollecteds[generation] = true;

		public void Dispose()
		{
			ResetStream();
			_platform.Dispose();
			Native.XtsSeqDSPDestroy(_nativeSeqDSP);
			Native.XtsSeqStateDestroy(_nativeSeqState);
			Native.XtsVoiceBindingDestroy(_nativeBinding);
			Native.XtsSynthModelDestroy(_nativeSynthModel);
		}

		internal void Stop()
		{
			if (_streamModel.IsRunning)
				PauseStream();
			else
				ResetStream();
		}

		internal void Start(SeqModel seq)
		{
			if (_streamModel.IsPaused)
				ResumeStream();
			else
				StartStream(seq);
		}

		void PauseStream()
		{
			try
			{
				_stream?.Stop();
				_originalSynthModel.CopyTo(_synthModel);
				_streamModel.State = StreamState.Paused;
			}
			catch
			{
				ResetStream();
				throw;
			}
		}

		void ResumeStream()
		{
			try
			{
				AutomationQueue.Clear();
				_synthModel.CopyTo(_originalSynthModel);
				_synthModel.ToNative(_nativeBinding);
				_synthModel.CopyTo(_managedSynthModel);
				_streamModel.State = StreamState.Running;
				_stream.Start();
			}
			catch
			{
				ResetStream();
				throw;
			}
		}

		void ResetStream()
		{
			PauseStream();

			Native.XtsSeqModelDestroy(_nativeSeqModel);
			_nativeSeqModel = null;
			_streamModel.State = StreamState.Stopped;
			_stream?.Dispose();
			_stream = null;
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

			_streamModel.Voices = 0;
			_streamModel.CpuUsage = 0.0;
			_streamModel.LatencyMs = 0.0;
			_streamModel.IsClipping = false;
			_streamModel.IsExhausted = false;
			_streamModel.IsOverloaded = false;
			_streamModel.GC0Collected = false;
			_streamModel.GC1Collected = false;
			_streamModel.GC2Collected = false;
			_streamModel.BufferSizeFrames = 0;

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
				var bufferSize = _settingsModel.BufferSize.ToInt();
				var streamParams = new XtStreamParams(true, OnXtBuffer, null, OnXtRunning);
				var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
				if (_settingsModel.WriteToDisk)
					_stream = new DiskStream(this, in format, bufferSize, _settingsModel.OutputPath);
				else
					_stream = OpenDeviceStream(in deviceParams);
				_cpuUsageIndex = 0;
				_cpuUsageTotalFrameCount = 0;
				_cpuUsageFactors = new double[format.mix.rate];
				_cpuUsageFrameCounts = new int[format.mix.rate];
				_buffer = (float*)Marshal.AllocHGlobal(_stream.GetMaxBufferFrames() * sizeof(float) * 2);
				UpdateStreamInfo(0, format.mix.rate, 0, false, false, 0);
				_nativeSeqModel = Native.XtsSeqModelCreate();
				seq.ToNative(_nativeSeqModel);
				Native.XtsSeqDSPInit(_nativeSeqDSP, _nativeSeqModel, _nativeSynthModel, _nativeBinding);
				ResumeStream();
			}
			catch
			{
				ResetStream();
				throw;
			}
		}

		void ResetWarnings(int rate, long streamPosition)
		{
			float infoFrames = InfoDurationSeconds * rate;
			if (streamPosition > _clipPosition + infoFrames)
				_streamModel.IsClipping = false;
			if (streamPosition > _gcPositions[0] + infoFrames)
				_streamModel.GC0Collected = false;
			if (streamPosition > _gcPositions[1] + infoFrames)
				_streamModel.GC1Collected = false;
			if (streamPosition > _gcPositions[2] + infoFrames)
				_streamModel.GC2Collected = false;
			if (streamPosition > _overloadPosition + infoFrames)
				_streamModel.IsOverloaded = false;
			if (streamPosition > _exhaustedPosition + infoFrames)
				_streamModel.IsExhausted = false;
		}

		void UpdateStreamInfo(int frames, int rate, int voices, bool clip, bool exhausted, long streamPosition)
		{
			float bufferSeconds = frames / (float)rate;
			var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
			if (clip)
			{
				_streamModel.IsClipping = true;
				_clipPosition = streamPosition;
			}
			if (exhausted)
			{
				_streamModel.IsExhausted = true;
				_exhaustedPosition = streamPosition;
			}
			if (_gcCollecteds[0])
			{
				_gcCollecteds[0] = false;
				_streamModel.GC0Collected = true;
				_gcPositions[0] = streamPosition;
			}
			if (_gcCollecteds[1])
			{
				_gcCollecteds[1] = false;
				_streamModel.GC1Collected = true;
				_gcPositions[1] = streamPosition;
			}
			if (_gcCollecteds[2])
			{
				_gcCollecteds[2] = false;
				_streamModel.GC2Collected = true;
				_gcPositions[2] = streamPosition;
			}
			if (processedSeconds > bufferSeconds * OverloadLimit)
			{
				_overloadPosition = streamPosition;
				_streamModel.IsOverloaded = true;
			}
			if (_bufferInfoPosition == -1 || streamPosition >=
				_bufferInfoPosition + rate * InfoDurationSeconds)
			{
				_bufferInfoPosition = streamPosition;
				_streamModel.LatencyMs = _stream.GetLatencyMs();
				_streamModel.BufferSizeFrames = _stream.GetMaxBufferFrames();
			}
			if (streamPosition >= _voiceInfoPosition + rate * InfoDurationSeconds)
			{
				_voiceInfoPosition = streamPosition;
				_streamModel.Voices = voices;
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
				_streamModel.CpuUsage = cpuUsage;
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
			var @params = _managedSynthModel.Params;
			var actions = AutomationQueue.DequeueUI(out var count);
			for (int i = 0; i < count; i++)
				@params[actions[i].Param].Value = @actions[i].Value;
			for (int i = 0; i < @params.Count; i++)
				_automationValues[i] = @params[i].Value;
			_managedSynthModel.ToNative(_nativeBinding);
		}

		void EndAutomation()
		{
			_managedSynthModel.FromNative(_nativeBinding);
			var @params = _managedSynthModel.Params;
			for (int i = 0; i < @params.Count; i++)
				if (@params[i].Value != _automationValues[i])
					AutomationQueue.EnqueueAudio(i, @params[i].Value);
		}

		internal unsafe void OnBuffer(in XtBuffer buffer, in XtFormat format)
		{
			_stopwatch.Restart();
			BeginAutomation();
			int rate = format.mix.rate;
			var state = _nativeSeqState;
			state->rate = rate;
			state->buffer = _buffer;
			state->frames = buffer.frames;
			state->seq = _nativeSeqModel;
			state->synth = _nativeSynthModel;
			Native.XtsSeqDSPRender(_nativeSeqDSP, state);
			EndAutomation();
			long pos = _nativeSeqState->pos;
			CopyBuffer(in buffer, in format);
			ResetWarnings(rate, pos);
			_streamModel.CurrentRow = _nativeSeqState->row;
			_stopwatch.Stop();
			UpdateCpuUsage(buffer.frames, rate, pos);
			bool clip = state->clip != 0;
			bool exhausted = state->exhausted != 0;
			UpdateStreamInfo(buffer.frames, rate, state->voices, clip, exhausted, pos);
			if (state->end != 0) _asyncStop();
		}

		int OnXtBuffer(XtStream stream, in XtBuffer buffer, object user)
		{
			OnBuffer(buffer, stream.GetFormat());
			return 0;
		}

		internal void OnRunning(bool running, ulong error)
		{
			if (!running && error != 0)
				_dispatchToUI(ResetStream);
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
			var depth = _settingsModel.BitDepth.ToInt();
			var rate = _settingsModel.SampleRate.ToInt();
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
			var system = _settingsModel.UseAsio ? XtSystem.ASIO : XtSystem.WASAPI;
			var selectedId = _settingsModel.UseAsio ? _settingsModel.AsioDeviceId : _settingsModel.WasapiDeviceId;
			var defaultId = _settingsModel.UseAsio ? AsioDefaultDeviceId : WasapiDefaultDeviceId;
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