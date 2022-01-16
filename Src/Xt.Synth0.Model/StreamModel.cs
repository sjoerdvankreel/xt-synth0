using System.ComponentModel;

namespace Xt.Synth0.Model
{
	public enum StreamState { Stopped, Paused, Running }

	public sealed class StreamModel : INotifyPropertyChanged
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

		static readonly PropertyChangedEventArgs VoicesChangedEventArgs
		= new PropertyChangedEventArgs(nameof(Voices));
		static readonly PropertyChangedEventArgs LatencyMsChangedEventArgs
		= new PropertyChangedEventArgs(nameof(LatencyMs));
		static readonly PropertyChangedEventArgs CpuUsageChangedEventArgs
		= new PropertyChangedEventArgs(nameof(CpuUsage));
		static readonly PropertyChangedEventArgs IsClippingChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsClipping));
		static readonly PropertyChangedEventArgs IsExhaustedChangedEventArgs
		= new PropertyChangedEventArgs(nameof(IsExhausted));
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

		public bool IsPaused => State == StreamState.Paused;
		public bool IsRunning => State == StreamState.Running;
		public bool IsStopped => State == StreamState.Stopped;

		StreamState _state;
		public StreamState State
		{
			get => _state;
			set
			{
				if (_state == value) return;
				bool wasPaused = _state == StreamState.Paused;
				bool wasStopped = _state == StreamState.Stopped;
				bool wasRunning = _state == StreamState.Running;
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

		int _voices;
		public int Voices
		{
			get => _voices;
			set
			{
				if (_voices == value) return;
				_voices = value;
				PropertyChanged?.Invoke(this, VoicesChangedEventArgs);
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

		bool _isExhausted;
		public bool IsExhausted
		{
			get => _isExhausted;
			set
			{
				if (_isExhausted == value) return;
				_isExhausted = value;
				PropertyChanged?.Invoke(this, IsExhaustedChangedEventArgs);
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