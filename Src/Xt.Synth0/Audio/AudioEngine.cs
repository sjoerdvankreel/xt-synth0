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
		const float WarningSeconds = 0.5f;

		static XtSample DepthToSample(int size) => size switch
		{
			16 => XtSample.Int16,
			24 => XtSample.Int24,
			32 => XtSample.Int32,
			_ => throw new InvalidOperationException()
		};

		internal static AudioEngine Create(
			AppModel app, IntPtr mainWindow, Action<string> log,
			Action<Action> dispatchToUI, Action<SynthModel> bufferFinished)
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
			Action<Action> dispatchToUI, Action<SynthModel> bufferFinished)
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

		int _rate;
		IAudioStream _stream;

		readonly AppModel _app;
		readonly SynthDSP _dsp = new();
		readonly SynthModel _original = new();

		readonly XtPlatform _platform;
		readonly Action<Action> _dispatchToUI;
		readonly Action<SynthModel> _bufferFinished;

		readonly ParamAction[] _autoActions;
		readonly SynthModel _beforeAutomation = new();

		long _clipPosition = -1;
		long _streamPosition = 0;
		long _overloadPosition = -1;
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
			Action<SynthModel> bufferFinished,
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
			_autoActions = new ParamAction[app.Synth.AutoParams().Count];
		}

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
				Array.Clear(_autoActions);
			}
			finally
			{
				_dsp.Reset(_app.Audio);
				_original.CopyTo(_app.Synth, true);
			}
		}

		void StartStream()
		{
			try
			{
				_dsp.Reset(_app.Audio);
				_app.Synth.CopyTo(_original, true);
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

			_rate = 0;
			_stopwatch.Reset();
			_clipPosition = -1;
			_streamPosition = 0;
			_overloadPosition = -1;
			_app.Audio.IsClipping = false;
			_app.Audio.IsOverloaded = false;
		}

		SynthModel PrepareModel()
		{
			var result = ModelPool.Get();
			_app.Synth.CopyTo(result, false);
			ApplyAutomation(result);
			result.CopyTo(_beforeAutomation, true);
			return result;
		}

		void UpdateOverloadWarning(int frames)
		{
			float bufferSeconds = frames / (float)_rate;
			var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
			if (processedSeconds > bufferSeconds * OverloadLimit)
			{
				_overloadPosition = _streamPosition;
				_app.Audio.IsOverloaded = true;
			}
		}

		void ResetWarnings()
		{
			float warningFrames = WarningSeconds * _rate;
			if (_streamPosition > _clipPosition + warningFrames)
				_app.Audio.IsClipping = false;
			if (_streamPosition > _overloadPosition + warningFrames)
				_app.Audio.IsOverloaded = false;
		}

		void UpdateAutomation(SynthModel synth)
		{
			var newAutos = synth.AutoParams();
			var oldAutos = _beforeAutomation.AutoParams();
			for (int a = 0; a < _autoActions.Length; a++)
			{
				int value = newAutos[a].Param.Value;
				_autoActions[a].Value = value;
				_autoActions[a].Changed = value != oldAutos[a].Param.Value;
			}
		}

		void ApplyAutomation(SynthModel synth)
		{
			for (int a = 0; a < _autoActions.Length; a++)
				if (_autoActions[a].Changed)
					synth.AutoParams()[a].Param.Value = _autoActions[a].Value;
		}

		void ProcessBuffer(SynthModel synth, int frames)
		{
			for (int f = 0; f < frames; f++)
			{
				ProcessFrame(synth, f);
				_streamPosition++;
			}
		}

		void ProcessFrame(SynthModel synth, int frame)
		{
			var sample = _dsp.Next(synth, _app.Audio, _rate) * MaxAmp;
			if (sample > MaxAmp)
			{
				_clipPosition = _streamPosition;
				_app.Audio.IsClipping = true;
			}
			sample = Math.Clamp(sample, -MaxAmp, MaxAmp);
			_buffer[frame * 2] = sample;
			_buffer[frame * 2 + 1] = sample;
		}

		void CopyBuffer(XtStream stream, in XtBuffer buffer)
		{
			var format = stream.GetFormat();
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

		int OnBuffer(XtStream stream, in XtBuffer buffer, object user)
		{
			_stopwatch.Restart();
			var synth = PrepareModel();
			ProcessBuffer(synth, buffer.frames);
			CopyBuffer(stream, in buffer);
			UpdateAutomation(synth);
			ResetWarnings();
			_stopwatch.Stop();
			UpdateOverloadWarning(buffer.frames);
			_bufferFinished(synth);
			return 0;
		}

		void OnRunning(XtStream stream, bool running, ulong error, object user)
		{
			if (!running && error != 0)
				_dispatchToUI(ResetStream);
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

		IAudioStream OpenStream(in XtDeviceStreamParams deviceParams)
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
			var streamParams = new XtStreamParams(true, OnBuffer, null, OnRunning);
			var bufferSize = AudioModel.BufferSizeToInt(_app.Settings.BufferSize);
			var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
			var result = OpenStream(in deviceParams);
			_stream = result;
			_rate = format.mix.rate;
			result.Start();
		}
	}
}