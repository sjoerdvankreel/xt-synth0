using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public static class AudioModel
	{
		public static int ToInt(this SampleRate rate) => rate switch
		{
			SampleRate.Rate44100 => 44100,
			SampleRate.Rate48000 => 48000,
			SampleRate.Rate96000 => 96000,
			SampleRate.Rate192000 => 192000,
			_ => throw new InvalidOperationException()
		};

		public static int ToInt(this BitDepth depth) => depth switch
		{
			BitDepth.Depth16 => 16,
			BitDepth.Depth24 => 24,
			BitDepth.Depth32 => 32,
			_ => throw new InvalidOperationException()
		};

		public static int ToInt(this BufferSize size) => size switch
		{
			BufferSize.Size1 => 1,
			BufferSize.Size2 => 2,
			BufferSize.Size3 => 3,
			BufferSize.Size5 => 5,
			BufferSize.Size10 => 10,
			BufferSize.Size20 => 20,
			BufferSize.Size30 => 30,
			BufferSize.Size50 => 50,
			BufferSize.Size100 => 100,
			_ => throw new InvalidOperationException()
		};

		public static IReadOnlyList<EnumModel<BitDepth>> BitDepths { get; }
			= new ReadOnlyCollection<EnumModel<BitDepth>>(Enum.GetValues<BitDepth>()
				.Select(d => new EnumModel<BitDepth>(d, d.ToInt())).ToList());

		public static IReadOnlyList<EnumModel<SampleRate>> SampleRates { get; }
			= new ReadOnlyCollection<EnumModel<SampleRate>>(Enum.GetValues<SampleRate>()
				.Select(r => new EnumModel<SampleRate>(r, r.ToInt())).ToList());

		public static IReadOnlyList<EnumModel<BufferSize>> BufferSizes { get; }
			= new ReadOnlyCollection<EnumModel<BufferSize>>(Enum.GetValues<BufferSize>()
				.Select(s => new EnumModel<BufferSize>(s, s.ToInt())).ToList());

		public static IReadOnlyList<EnumModel<DeviceType>> DeviceTypes { get; }
			= new ReadOnlyCollection<EnumModel<DeviceType>>(Enum.GetValues<DeviceType>()
				.Select(t => new EnumModel<DeviceType>(t, (int)t)).ToList());

		static readonly List<DeviceModel> _asioDevices = new();
		static readonly List<DeviceModel> _wasapiDevices = new();
		static readonly List<DeviceModel> _dSoundDevices = new();
		public static void AddAsioDevices(IEnumerable<DeviceModel> models) => _asioDevices.AddRange(models);
		public static void AddWasapiDevices(IEnumerable<DeviceModel> models) => _wasapiDevices.AddRange(models);
		public static void AddDSoundDevices(IEnumerable<DeviceModel> models) => _dSoundDevices.AddRange(models);
		public static IReadOnlyList<DeviceModel> AsioDevices { get; } = new ReadOnlyCollection<DeviceModel>(_asioDevices);
		public static IReadOnlyList<DeviceModel> WasapiDevices { get; } = new ReadOnlyCollection<DeviceModel>(_wasapiDevices);
		public static IReadOnlyList<DeviceModel> DSoundDevices { get; } = new ReadOnlyCollection<DeviceModel>(_dSoundDevices);
	}
}