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
		static readonly PropertyChangedEventArgs IsClippingChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsClipping));
		static readonly PropertyChangedEventArgs IsOverloadedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsOverloaded));

		public event PropertyChangedEventHandler PropertyChanged;

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

		public bool IsPaused => State == AudioState.Paused;
		public bool IsRunning => State == AudioState.Running;
		public bool IsStopped => State == AudioState.Stopped;

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
	}
}