using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

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

        readonly bool _notify;
        public StreamModel(bool notify) => _notify = notify;

        void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!_notify) return;
            CheckThread();
            PropertyChanged?.Invoke(this, e);
        }

        [Conditional("DEBUG")]
        void CheckThread()
        {
            if (Model.MainThreadId != Thread.CurrentThread.ManagedThreadId)
                throw new InvalidOperationException();
        }

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
                OnPropertyChanged(StateChangedEventArgs);
                if (IsPaused != wasPaused) OnPropertyChanged(IsPausedChangedEventArgs);
                if (IsStopped != wasStopped) OnPropertyChanged(IsStoppedChangedEventArgs);
                if (IsRunning != wasRunning) OnPropertyChanged(IsRunningChangedEventArgs);
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
                OnPropertyChanged(VoicesChangedEventArgs);
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
                OnPropertyChanged(CpuUsageChangedEventArgs);
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
                OnPropertyChanged(CurrentRowChangedEventArgs);
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
                OnPropertyChanged(LatencyMsChangedEventArgs);
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
                OnPropertyChanged(IsClippingChangedEventArgs);
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
                OnPropertyChanged(IsExhaustedChangedEventArgs);
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
                OnPropertyChanged(IsOverloadedChangedEventArgs);
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
                OnPropertyChanged(GC0CollectedChangedEventArgs);
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
                OnPropertyChanged(GC1CollectedChangedEventArgs);
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
                OnPropertyChanged(GC2CollectedChangedEventArgs);
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
                OnPropertyChanged(BufferSizeFramesChangedEventArgs);
            }
        }

        public void CopyTo(StreamModel stream)
        {
            stream.State = State;
            stream.Voices = Voices;
            stream.CpuUsage = CpuUsage;
            stream.LatencyMs = LatencyMs;
            stream.CurrentRow = CurrentRow;
            stream.IsClipping = IsClipping;
            stream.IsExhausted = IsExhausted;
            stream.IsOverloaded = IsOverloaded;
            stream.GC0Collected = GC0Collected;
            stream.GC1Collected = GC1Collected;
            stream.GC2Collected = GC2Collected;
            stream.BufferSizeFrames = BufferSizeFrames;
        }
    }
}