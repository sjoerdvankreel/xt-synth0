using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xt.Synth0.Model
{
	public sealed class AudioModel : ViewModel
	{
		static readonly IList<DeviceModel> _asioDevices = new List<DeviceModel>();
		static readonly IList<DeviceModel> _wasapiDevices = new List<DeviceModel>();

		public static void AddAsioDevice(string id, string name)
		=> _asioDevices.Add(new DeviceModel(id, name));
		public static void AddWasapiDevice(string id, string name)
		=> _wasapiDevices.Add(new DeviceModel(id, name));

		public static IReadOnlyList<DeviceModel> AsioDevices { get; }
			= new ReadOnlyCollection<DeviceModel>(_asioDevices);
		public static IReadOnlyList<DeviceModel> WasapiDevices { get; }
			= new ReadOnlyCollection<DeviceModel>(_wasapiDevices);

		public static IReadOnlyList<RateModel> SampleRates { get; }
			= new ReadOnlyCollection<RateModel>(
				new List<RateModel>(new RateModel[] {
					new RateModel(SampleRate.Rate44100, "44100"),
					new RateModel(SampleRate.Rate48000, "48000"),
					new RateModel(SampleRate.Rate96000, "96000") }));

		bool _isRunning;
		public bool IsRunning
		{
			get => _isRunning;
			set => Set(ref _isRunning, value);
		}
	}
}