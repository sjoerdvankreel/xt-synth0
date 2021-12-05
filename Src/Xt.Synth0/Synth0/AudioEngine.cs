using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
	class AudioEngine : IDisposable
	{
		static AudioEngine Create(XtPlatform platform)
		{
			var asio = platform.GetService(XtSystem.ASIO);
			var wasapi = platform.GetService(XtSystem.WASAPI);
			return new AudioEngine(platform,
				asio.GetDefaultDeviceId(true),
				wasapi.GetDefaultDeviceId(true),
				GetDevices(asio),
				GetDevices(wasapi));
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

		internal static AudioEngine Create(IntPtr mainWindow)
		{
			var platform = XtAudio.Init(nameof(Synth0), mainWindow);
			try
			{
				return Create(platform);
			}
			catch
			{
				platform.Dispose();
				throw;
			}
		}

		readonly XtPlatform _platform;
		public string AsioDefaultDeviceId { get; }
		public string WasapiDefaultDeviceId { get; }
		public IReadOnlyList<DeviceModel> AsioDevices { get; }
		public IReadOnlyList<DeviceModel> WasapiDevices { get; }

		AudioEngine(XtPlatform platform,
			string asioDefaultDeviceId,
			string wasapiDefaultDeviceId,
			IReadOnlyList<DeviceModel> asioDevices,
			IReadOnlyList<DeviceModel> wasapiDevices)
		{
			_platform = platform;
			AsioDevices = asioDevices;
			WasapiDevices = wasapiDevices;
			AsioDefaultDeviceId = asioDefaultDeviceId;
			WasapiDefaultDeviceId = wasapiDefaultDeviceId;
		}

		public void Dispose() => _platform.Dispose();
	}
}