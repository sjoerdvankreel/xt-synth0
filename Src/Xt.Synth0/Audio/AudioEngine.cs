using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xt.Synth0.DSP;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class AudioEngine : IDisposable
	{
		const float MaxAmp = 0.9f;
		const float OverloadLimit = 0.9f;
		const float WarningDurationSeconds = 0.5f;
		const float CpuUsageIntervalSeconds = 0.5f;
		const float BufferInfoIntervalSeconds = 1.0f;
		const float GcNotificationDurationSeconds = 0.2f;

		static XtSample DepthToSample(int size) => size switch
		{
			16 => XtSample.Int16,
			24 => XtSample.Int24,
			32 => XtSample.Int32,
			_ => throw new InvalidOperationException()
		};

		internal static AudioEngine Create(
			AppModel app, IntPtr mainWindow, Action<string> log,
			Action<Action> dispatchToUI, Action<AutomationEntry> bufferFinished)
		{
			XtAudio.SetOnError(msg => log(msg));
			var platform = XtAudio.Init(nameof(Synth0), mainWindow);
			try
			{
				return Create(app, platform, dispatchToUI, bufferFinished);
			}
			catch
			{
				platform.Dispose();
				throw;
			}
		}

		static AudioEngine Create(AppModel app, XtPlatform platform,
			Action<Action> dispatchToUI, Action<AutomationEntry> bufferFinished)
		{
			var asio = platform.GetService(XtSystem.ASIO);
			var wasapi = platform.GetService(XtSystem.WASAPI);
			return new AudioEngine(app, platform,
				dispatchToUI, bufferFinished,
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

		readonly AppModel _app;
		readonly SynthDSP _dsp = new();
		readonly SynthModel _original = new();

		readonly XtPlatform _platform;
		readonly Action<Action> _dispatchToUI;
		readonly Action<AutomationEntry> _bufferFinished;

		IAudioStream _stream;
		long _clipPosition = -1;
		long _streamPosition = 0;
		long _overloadPosition = -1;
		long _cpuUsagePosition = -1;
		long _bufferInfoPosition = -1;
		readonly long[] _gcPositions = new long[3];
		readonly bool[] _gcCollecteds = new bool[3];
		readonly Stopwatch _stopwatch = new Stopwatch();
		readonly float[] _buffer = new float[192000 * 2];

		public string AsioDefaultDeviceId { get; }
		public string WasapiDefaultDeviceId { get; }
		public IReadOnlyList<DeviceModel> AsioDevices { get; }
		public IReadOnlyList<DeviceModel> WasapiDevices { get; }

		AudioEngine(
			AppModel app,
			XtPlatform platform,
			Action<Action> dispatchToUI,
			Action<AutomationEntry> bufferFinished,
			string asioDefaultDeviceId,
			string wasapiDefaultDeviceId,
			IReadOnlyList<DeviceModel> asioDevices,
			IReadOnlyList<DeviceModel> wasapiDevices)
		{
			AsioDevices = asioDevices;
			WasapiDevices = wasapiDevices;
			AsioDefaultDeviceId = asioDefaultDeviceId;
			WasapiDefaultDeviceId = wasapiDefaultDeviceId;

			_app = app;
			_platform = platform;
			_dispatchToUI = dispatchToUI;
			_bufferFinished = bufferFinished;
			GCNotification.Register(this);
		}

		internal void OnGCNotification(int generation)
		=> _gcCollecteds[generation] = true;

		public void Dispose()
		{
			ResetStream();
			_platform.Dispose();
		}

		internal void Stop()
		{
			if (_app.Audio.IsRunning)
				PauseStream();
			else
				ResetStream();
		}

		internal void Start()
		{
			if (_app.Audio.IsPaused)
				ResumeStream();
			else
				StartStream();
		}

		void PauseStream()
		{
			try
			{
				_app.Audio.State = AudioState.Paused;
				_stream.Stop();
			}
			catch
			{
				ResetStream();
				throw;
			}
		}

		void ResetStream()
		{
			try
			{
				_app.Audio.State = AudioState.Stopped;
				DoResetStream();
			}
			finally
			{
				_dsp.Reset(_app.Audio);
				_original.CopyTo(_app.Track.Synth);
			}
		}

		void StartStream()
		{
			try
			{
				_dsp.Reset(_app.Audio);
				_app.Track.Synth.CopyTo(_original);
				_app.Audio.State = AudioState.Running;
				DoStartStream();
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
				_app.Audio.State = AudioState.Running;
				_stream.Start();
			}
			catch
			{
				ResetStream();
				throw;
			}
		}

		void DoResetStream()
		{
			_stream?.Dispose();
			_stream = null;
			_stopwatch.Reset();

			_clipPosition = -1;
			_streamPosition = 0;
			_overloadPosition = -1;
			_cpuUsagePosition = -1;
			_bufferInfoPosition = -1;

			_app.Audio.CpuUsage = 0.0;
			_app.Audio.LatencyMs = 0.0;
			_app.Audio.IsClipping = false;
			_app.Audio.IsOverloaded = false;
			_app.Audio.GC0Collected = false;
			_app.Audio.GC1Collected = false;
			_app.Audio.GC2Collected = false;
			_app.Audio.BufferSizeFrames = 0;

			for (int i = 0; i < _gcPositions.Length; i++)
			{
				_gcPositions[i] = -1;
				_gcCollecteds[i] = false;
			}
		}

		void ProcessBuffer(AutomationEntry entry, int frames, int rate)
		{
			for (int f = 0; f < frames; f++)
			{
				ProcessFrame(entry, f, rate);
				_streamPosition++;
			}
		}

		void ProcessFrame(AutomationEntry entry, int frame, int rate)
		{
			var seq = _app.Track.Sequencer;
			var sample = _dsp.Next(entry.Model, seq, _app.Audio, rate, entry.Automated) * MaxAmp;
			if (sample > MaxAmp)
			{
				_clipPosition = _streamPosition;
				_app.Audio.IsClipping = true;
			}
			sample = Math.Clamp(sample, -MaxAmp, MaxAmp);
			_buffer[frame * 2] = sample;
			_buffer[frame * 2 + 1] = sample;
		}

		void ResetWarnings(int rate)
		{
			float warningFrames = WarningDurationSeconds * rate;
			float gcNotificationFrames = GcNotificationDurationSeconds * rate;
			if (_streamPosition > _clipPosition + warningFrames)
				_app.Audio.IsClipping = false;
			if (_streamPosition > _overloadPosition + warningFrames)
				_app.Audio.IsOverloaded = false;
			if (_streamPosition > _gcPositions[0] + gcNotificationFrames)
				_app.Audio.GC0Collected = false;
			if (_streamPosition > _gcPositions[1] + gcNotificationFrames)
				_app.Audio.GC1Collected = false;
			if (_streamPosition > _gcPositions[2] + gcNotificationFrames)
				_app.Audio.GC2Collected = false;
		}

		void UpdateStreamInfo(int frames, int rate)
		{
			float bufferSeconds = frames / (float)rate;
			var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
			if (_gcCollecteds[0])
			{
				_gcCollecteds[0] = false;
				_app.Audio.GC0Collected = true;
				_gcPositions[0] = _streamPosition;
			}
			if (_gcCollecteds[1])
			{
				_gcCollecteds[1] = false;
				_app.Audio.GC1Collected = true;
				_gcPositions[1] = _streamPosition;
			}
			if (_gcCollecteds[2])
			{
				_gcCollecteds[2] = false;
				_app.Audio.GC2Collected = true;
				_gcPositions[2] = _streamPosition;
			}
			if (processedSeconds > bufferSeconds * OverloadLimit)
			{
				_overloadPosition = _streamPosition;
				_app.Audio.IsOverloaded = true;
			}
			if (_streamPosition >= _cpuUsagePosition + rate * CpuUsageIntervalSeconds)
			{
				_cpuUsagePosition = _streamPosition;
				_app.Audio.CpuUsage = Math.Min(processedSeconds / bufferSeconds, 1.0);
			}
			if (_bufferInfoPosition == -1 || _streamPosition >= 
				_bufferInfoPosition + rate * BufferInfoIntervalSeconds)
			{
				_bufferInfoPosition = _streamPosition;
				_app.Audio.LatencyMs = _stream.GetLatencyMs();
				_app.Audio.BufferSizeFrames = _stream.GetMaxBufferFrames();
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

		internal void OnBuffer(in XtBuffer buffer, in XtFormat format)
		{
			_stopwatch.Restart();
			var entry = AutomationPool.Get();
			Array.Clear(entry.Automated);
			_app.Track.Synth.CopyTo(entry.Model);
			int rate = format.mix.rate;
			ProcessBuffer(entry, buffer.frames, rate);
			CopyBuffer(in buffer, in format);
			ResetWarnings(rate);
			_stopwatch.Stop();
			UpdateStreamInfo(buffer.frames, rate);
			_bufferFinished(entry);
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
			var rate = AudioModel.RateToInt(_app.Settings.SampleRate);
			var depth = AudioModel.BitDepthToInt(_app.Settings.BitDepth);
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
			var model = _app.Settings;
			var system = model.UseAsio ? XtSystem.ASIO : XtSystem.WASAPI;
			var selectedId = model.UseAsio ? model.AsioDeviceId : model.WasapiDeviceId;
			var defaultId = model.UseAsio ? AsioDefaultDeviceId : WasapiDefaultDeviceId;
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

		void DoStartStream()
		{
			var format = GetFormat();
			var settings = _app.Settings;
			var streamParams = new XtStreamParams(true, OnXtBuffer, null, OnXtRunning);
			var bufferSize = AudioModel.BufferSizeToInt(settings.BufferSize);
			var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
			if (settings.WriteToDisk)
				_stream = new DiskStream(this, in format, bufferSize, settings.OutputPath);
			else
				_stream = OpenDeviceStream(in deviceParams);
			UpdateStreamInfo(0, format.mix.rate);
			GC.Collect(2, GCCollectionMode.Forced, true, true);
			_stream.Start();
		}
	}
}