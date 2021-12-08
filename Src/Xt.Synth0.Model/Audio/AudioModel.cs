using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class AudioModel : ViewModel
	{
		public static int RateToInt(SampleRate rate) => rate switch
		{
			SampleRate.Rate44100 => 44100,
			SampleRate.Rate48000 => 48000,
			SampleRate.Rate96000 => 96000,
			_ => throw new InvalidOperationException()
		};

		public static int SizeToInt(BufferSize size) => size switch
		{
			BufferSize.Size3 => 3,
			BufferSize.Size5 => 5,
			BufferSize.Size10 => 10,
			BufferSize.Size30 => 30,
			BufferSize.Size50 => 50,
			BufferSize.Size100 => 100,
			_ => throw new InvalidOperationException()
		};

		public static IReadOnlyList<RateModel> SampleRates { get; }
			= new ReadOnlyCollection<RateModel>(Enum.GetValues<SampleRate>()
				.Select(r => new RateModel(r, RateToInt(r))).ToList());

		public static IReadOnlyList<BufferModel> BufferSizes { get; }
			= new ReadOnlyCollection<BufferModel>(Enum.GetValues<BufferSize>()
				.Select(s => new BufferModel(s, SizeToInt(s))).ToList());

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

		public event EventHandler RowChanged;

		bool _isRunning;
		public bool IsRunning
		{
			get => _isRunning;
			set => Set(ref _isRunning, value);
		}

		int _currentRow = -1;
		public int CurrentRow
		{
			get => _currentRow;
			set
			{
				int oldRow = _currentRow;
				Set(ref _currentRow, value);
				if (oldRow != value)
					RowChanged(this, EventArgs.Empty);
			}
		}
	}
}