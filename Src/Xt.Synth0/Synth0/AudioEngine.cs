using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xt.Synth0.DSP;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class AudioEngine : IDisposable
	{
		internal static AudioEngine Create(AppModel app,
			IntPtr mainWindow, Action<string> log, Action<Action> dispatch)
		{
			XtAudio.SetOnError(msg => log(msg));
			var platform = XtAudio.Init(nameof(Synth0), mainWindow);
			try
			{
				return Create(app, platform, dispatch);
			}
			catch
			{
				platform.Dispose();
				throw;
			}
		}

		static AudioEngine Create(AppModel app,
			XtPlatform platform, Action<Action> dispatch)
		{
			var asio = platform.GetService(XtSystem.ASIO);
			var wasapi = platform.GetService(XtSystem.WASAPI);
			return new AudioEngine(app, platform, dispatch,
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
		readonly SynthModel _synth = new();

		readonly XtPlatform _platform;
		readonly Action<Action> _dispatch;
		readonly Action _stopNotification;
		readonly Action _startNotification;

		public string AsioDefaultDeviceId { get; }
		public string WasapiDefaultDeviceId { get; }
		public IReadOnlyList<DeviceModel> AsioDevices { get; }
		public IReadOnlyList<DeviceModel> WasapiDevices { get; }

		AudioEngine(
			AppModel app,
			XtPlatform platform,
			Action<Action> dispatch,
			string asioDefaultDeviceId,
			string wasapiDefaultDeviceId,
			IReadOnlyList<DeviceModel> asioDevices,
			IReadOnlyList<DeviceModel> wasapiDevices)
		{
			_app = app;
			_platform = platform;
			_dispatch = dispatch;
			AsioDevices = asioDevices;
			WasapiDevices = wasapiDevices;
			AsioDefaultDeviceId = asioDefaultDeviceId;
			WasapiDefaultDeviceId = wasapiDefaultDeviceId;
			_stopNotification = Stop;
			_startNotification = () => _app.Audio.IsRunning = true;
		}

		internal void Start(SettingsModel model)
		{
			Stop();
			try
			{
				Run(model);
			}
			catch
			{
				Stop();
				throw;
			}
		}

		internal void Stop()
		{
			_stream?.Stop();
			_app.Audio.IsRunning = false;
			_stream?.Dispose();
			_stream = null;
			_safe?.Dispose();
			_safe = null;
			_rate = 0;
			_device?.Dispose();
			_device = null;
		}

		public void Dispose()
		{
			Stop();
			_platform.Dispose();
		}

		int OnBuffer(XtStream stream, in XtBuffer buffer, object user)
		{
			_app.Synth.CopyTo(_synth);
			var safe = XtSafeBuffer.Get(stream);
			safe.Lock(buffer);
			var output = (float[])safe.GetOutput();
			_dsp.Next(_synth, _app.Audio, _rate, output, buffer.frames);
			safe.Unlock(buffer);
			return 0;
		}

		void OnRunning(XtStream stream, bool running, ulong error, object user)
		{
			if (running) _dispatch(_startNotification);
			else _dispatch(_stopNotification);
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

		void Run(SettingsModel model)
		{
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