using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class AudioEngine : IDisposable
	{
		internal static AudioEngine Create(IntPtr mainWindow,
			Action<string> log, Action<Action> dispatch)
		{
			XtAudio.SetOnError(msg => log(msg));
			var platform = XtAudio.Init(nameof(Synth0), mainWindow);
			try
			{
				return Create(platform, dispatch);
			}
			catch
			{
				platform.Dispose();
				throw;
			}
		}

		static AudioEngine Create(
			XtPlatform platform, Action<Action> dispatch)
		{
			var asio = platform.GetService(XtSystem.ASIO);
			var wasapi = platform.GetService(XtSystem.WASAPI);
			return new AudioEngine(platform, dispatch,
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

		XtDevice _device;
		XtStream _stream;
		readonly XtPlatform _platform;
		readonly Action<Action> _dispatch;
		readonly Action _stopNotification;
		readonly Action _startNotification;

		public event EventHandler Started;
		public event EventHandler Stopped;

		public string AsioDefaultDeviceId { get; }
		public string WasapiDefaultDeviceId { get; }
		public IReadOnlyList<DeviceModel> AsioDevices { get; }
		public IReadOnlyList<DeviceModel> WasapiDevices { get; }

		AudioEngine(XtPlatform platform,
			Action<Action> dispatch,
			string asioDefaultDeviceId,
			string wasapiDefaultDeviceId,
			IReadOnlyList<DeviceModel> asioDevices,
			IReadOnlyList<DeviceModel> wasapiDevices)
		{
			_platform = platform;
			_dispatch = dispatch;
			AsioDevices = asioDevices;
			WasapiDevices = wasapiDevices;
			AsioDefaultDeviceId = asioDefaultDeviceId;
			WasapiDefaultDeviceId = wasapiDefaultDeviceId;
			_stopNotification = StopNotification;
			_startNotification = () => Started?.Invoke(this, null);
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
			_stream?.Dispose();
			_stream = null;
			_device?.Dispose();
			_device = null;
		}

		public void Dispose()
		{
			Stop();
			_platform.Dispose();
		}

		void StopNotification()
		{
			Stop();
			Stopped?.Invoke(this, null);
		}

		int OnBuffer(XtStream stream, in XtBuffer buffer, object user)
		{
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
			var rate = AudioModel.RateToInt(model.SampleRate);
			var mix = new XtMix(rate, XtSample.Int16);
			var channels = new XtChannels(0, 0, 2, 0);
			var format = new XtFormat(in mix, in channels);
			var streamParams = new XtStreamParams(true, OnBuffer, null, OnRunning);
			var bufferSize = AudioModel.SizeToInt(model.BufferSize);
			var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
			_stream = _device.OpenStream(in deviceParams, null);
			_stream.Start();
		}
	}
}