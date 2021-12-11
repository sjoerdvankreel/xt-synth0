using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xt.Synth0.DSP;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class AudioEngine : IDisposable
	{
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
		XtDevice _device;
		XtStream _stream;
		XtSafeBuffer _safe;

		readonly AppModel _app;
		readonly SynthDSP _dsp = new();
		readonly SynthModel _original = new();

		readonly int[] _autoValues;
		readonly bool[] _automated;
		readonly SynthModel _beforeAutomation = new();

		readonly XtPlatform _platform;
		readonly Action<Action> _dispatchToUI;
		readonly Action<SynthModel> _bufferFinished;

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
			var autos = app.Synth.AutoParams().Count;

			_app = app;
			_platform = platform;
			_autoValues = new int[autos];
			_automated = new bool[autos];
			_dispatchToUI = dispatchToUI;
			_bufferFinished = bufferFinished;
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
				Array.Clear(_automated);
				Array.Clear(_autoValues);
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
			_stream?.Stop();
			_stream?.Dispose();
			_stream = null;
			_safe?.Dispose();
			_safe = null;
			_rate = 0;
			_device?.Dispose();
			_device = null;
		}

		void ApplyAutomation(SynthModel synth)
		{
			for (int a = 0; a < _automated.Length; a++)
				if (_automated[a])
					synth.AutoParams()[a].Param.Value = _autoValues[a];
		}

		void UpdateAutomation(SynthModel synth)
		{
			var newAutos = synth.AutoParams();
			var oldAutos = _beforeAutomation.AutoParams();
			for (int a = 0; a < _automated.Length; a++)
			{
				_autoValues[a] = newAutos[a].Param.Value;
				_automated[a] = _autoValues[a] != oldAutos[a].Param.Value;
			}
		}

		int OnBuffer(XtStream stream, in XtBuffer buffer, object user)
		{
			var synth = ModelPool.Get();
			_app.Synth.CopyTo(synth, false);
			ApplyAutomation(synth);
			synth.CopyTo(_beforeAutomation, true);
			var safe = XtSafeBuffer.Get(stream);
			safe.Lock(buffer);
			var output = (float[])safe.GetOutput();
			_dsp.Next(synth, _app.Audio, _rate, output, buffer.frames);
			safe.Unlock(buffer);
			UpdateAutomation(synth);
			_bufferFinished(synth);
			return 0;
		}

		void OnRunning(XtStream stream, bool running, ulong error, object user)
		{
			if (!running && error != 0)
				_dispatchToUI(ResetStream);
		}

		XtDevice OpenDevice(XtSystem system, string deviceId, string defaultId)
		{
			var service = _platform.GetService(system);
			var id = string.IsNullOrEmpty(deviceId) ? defaultId : deviceId;
			return service.OpenDevice(id);
		}

		internal void ShowASIOControlPanel(string deviceId)
		{
			using var device = OpenDevice(XtSystem.ASIO, deviceId, AsioDefaultDeviceId);
			device.ShowControlPanel();
		}

		void DoStartStream()
		{
			var model = _app.Settings;
			var system = model.UseAsio ? XtSystem.ASIO : XtSystem.WASAPI;
			var selectedId = model.UseAsio ? model.AsioDeviceId : model.WasapiDeviceId;
			var defaultId = model.UseAsio ? AsioDefaultDeviceId : WasapiDefaultDeviceId;
			_device = OpenDevice(system, selectedId, defaultId);
			_rate = AudioModel.RateToInt(model.SampleRate);
			var mix = new XtMix(_rate, XtSample.Float32);
			var channels = new XtChannels(0, 0, 2, 0);
			var format = new XtFormat(in mix, in channels);
			var streamParams = new XtStreamParams(true, OnBuffer, null, OnRunning);
			var bufferSize = AudioModel.SizeToInt(model.BufferSize);
			var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
			_stream = _device.OpenStream(in deviceParams, null);
			_safe = XtSafeBuffer.Register(_stream, true);
			_stream.Start();
		}
	}
}