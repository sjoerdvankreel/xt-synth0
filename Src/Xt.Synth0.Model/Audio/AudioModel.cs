using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class AudioModel : INotifyPropertyChanged
	{
		static readonly PropertyChangedEventArgs StateChangedEventArgs
		= new PropertyChangedEventArgs(nameof(State));
		static readonly PropertyChangedEventArgs IsPausedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsPaused));
		static readonly PropertyChangedEventArgs IsStoppedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsStopped));
		static readonly PropertyChangedEventArgs IsRunningChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsRunning));
		static readonly PropertyChangedEventArgs CurrentRowChangedEventArgs
		= new PropertyChangedEventArgs(nameof(CurrentRow));

		static readonly PropertyChangedEventArgs LatencyMsChangedEventArgs
		= new PropertyChangedEventArgs(nameof(LatencyMs));
		static readonly PropertyChangedEventArgs CpuUsageChangedEventArgs
		= new PropertyChangedEventArgs(nameof(CpuUsage));
		static readonly PropertyChangedEventArgs IsClippingChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsClipping));
		static readonly PropertyChangedEventArgs IsOverloadedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsOverloaded));
		static readonly PropertyChangedEventArgs GC0CollectedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(GC0Collected));
		static readonly PropertyChangedEventArgs GC1CollectedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(GC1Collected));
		static readonly PropertyChangedEventArgs GC2CollectedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(GC2Collected));
		static readonly PropertyChangedEventArgs BufferSizeFramesChangedEventArgs
		= new PropertyChangedEventArgs(nameof(BufferSizeFrames));

		public event PropertyChangedEventHandler PropertyChanged;

		public static int RateToInt(SampleRate rate) => rate switch
		{
			SampleRate.Rate44100 => 44100,
			SampleRate.Rate48000 => 48000,
			SampleRate.Rate96000 => 96000,
			SampleRate.Rate192000 => 192000,
			_ => throw new InvalidOperationException()
		};

		public static int BitDepthToInt(BitDepth depth) => depth switch
		{
			BitDepth.Depth16 => 16,
			BitDepth.Depth24 => 24,
			BitDepth.Depth32 => 32,
			_ => throw new InvalidOperationException()
		};

		public static int BufferSizeToInt(BufferSize size) => size switch
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

		public static IReadOnlyList<RateModel> SampleRates { get; }
			= new ReadOnlyCollection<RateModel>(Enum.GetValues<SampleRate>()
				.Select(r => new RateModel(r, RateToInt(r))).ToList());

		public static IReadOnlyList<DepthModel> BitDepths { get; }
			= new ReadOnlyCollection<DepthModel>(Enum.GetValues<BitDepth>()
				.Select(s => new DepthModel(s, BitDepthToInt(s))).ToList());

		public static IReadOnlyList<BufferModel> BufferSizes { get; }
			= new ReadOnlyCollection<BufferModel>(Enum.GetValues<BufferSize>()
				.Select(s => new BufferModel(s, BufferSizeToInt(s))).ToList());

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

		public bool IsPaused => State == AudioState.Paused;
		public bool IsRunning => State == AudioState.Running;
		public bool IsStopped => State == AudioState.Stopped;

		AudioState _state;
		public AudioState State
		{
			get => _state;
			set
			{
				if (_state == value) return;
				bool wasPaused = _state == AudioState.Paused;
				bool wasStopped = _state == AudioState.Stopped;
				bool wasRunning = _state == AudioState.Running;
				_state = value;
				PropertyChanged?.Invoke(this, StateChangedEventArgs);
				if (IsPaused != wasPaused)
					PropertyChanged?.Invoke(this, IsPausedChangedEventArgs);
				if (IsStopped != wasStopped)
					PropertyChanged?.Invoke(this, IsStoppedChangedEventArgs);
				if (IsRunning != wasRunning)
					PropertyChanged?.Invoke(this, IsRunningChangedEventArgs);
			}
		}

		int _currentRow;
		public int CurrentRow
		{
			get => _currentRow;
			set
			{
				if (_currentRow == value) return;
				_currentRow = value;
				PropertyChanged?.Invoke(this, CurrentRowChangedEventArgs);
			}
		}

		double _cpuUsage;
		public double CpuUsage
		{
			get => _cpuUsage;
			set
			{
				if (_cpuUsage == value) return;
				_cpuUsage = value;
				PropertyChanged?.Invoke(this, CpuUsageChangedEventArgs);
			}
		}

		double _latencyMs;
		public double LatencyMs
		{
			get => _latencyMs;
			set
			{
				if (_latencyMs == value) return;
				_latencyMs = value;
				PropertyChanged?.Invoke(this, LatencyMsChangedEventArgs);
			}
		}

		bool _isClipping;
		public bool IsClipping
		{
			get => _isClipping;
			set
			{
				if (_isClipping == value) return;
				_isClipping = value;
				PropertyChanged?.Invoke(this, IsClippingChangedEventArgs);
			}
		}

		bool _isOverloaded;
		public bool IsOverloaded
		{
			get => _isOverloaded;
			set
			{
				if (_isOverloaded == value) return;
				_isOverloaded = value;
				PropertyChanged?.Invoke(this, IsOverloadedChangedEventArgs);
			}
		}

		bool _gc0Collected = false;
		public bool GC0Collected
		{
			get => _gc0Collected;
			set
			{
				if (_gc0Collected == value) return;
				_gc0Collected = value;
				PropertyChanged?.Invoke(this, GC0CollectedChangedEventArgs);
			}
		}

		bool _gc1Collected = false;
		public bool GC1Collected
		{
			get => _gc1Collected;
			set
			{
				if (_gc1Collected == value) return;
				_gc1Collected = value;
				PropertyChanged?.Invoke(this, GC1CollectedChangedEventArgs);
			}
		}

		bool _gc2Collected = false;
		public bool GC2Collected
		{
			get => _gc2Collected;
			set
			{
				if (_gc2Collected == value) return;
				_gc2Collected = value;
				PropertyChanged?.Invoke(this, GC2CollectedChangedEventArgs);
			}
		}

		int _bufferSizeFrames;
		public int BufferSizeFrames
		{
			get => _bufferSizeFrames;
			set
			{
				if (_bufferSizeFrames == value) return;
				_bufferSizeFrames = value;
				PropertyChanged?.Invoke(this, BufferSizeFramesChangedEventArgs);
			}
		}
	}
}