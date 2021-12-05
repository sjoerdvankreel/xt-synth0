using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xt.Synth0.Model
{
	public sealed class AudioModel : ViewModel
	{
		static readonly List<DeviceModel> _asioDevices = new();
		static readonly List<DeviceModel> _wasapiDevices = new();
		public static void AddAsioDevices(IEnumerable<DeviceModel> models) 
		=> _asioDevices.AddRange(models);
		public static void AddWasapiDevices(IEnumerable<DeviceModel> models) 
		=> _wasapiDevices.AddRange(models);

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