using System;
using System.Diagnostics;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
    unsafe class AudioMonitor
    {
        struct Flag
        {
            internal bool Value;
            internal long Position;

            internal void Clear()
            {
                Value = false;
                Position = 0;
            }

            internal void Update(bool value, long position)
            {
                if (!value) return;
                Value = true;
                Position = position;
            }

            internal void Reset(int rate, long position)
            {
                float frames = InfoDurationSeconds * rate;
                if (position > Position + frames) Value = false;
            }
        }

        const float OverloadLimit = 0.9f;
        const float InfoDurationSeconds = 0.5f;
        const float CpuUsageUpdateIntervalSeconds = 1.0f;
        const float CpuUsageSamplingPeriodSeconds = 0.5f;

        int _cpuUsageIndex;
        double[] _cpuUsageFactors;
        int[] _cpuUsageFrameCounts;
        int _cpuUsageTotalFrameCount;
        readonly StreamModel _localStream = new(false);

        Flag _clip;
        Flag _overload;
        Flag _exhausted;
        Flag[] _gc = new Flag[3];

        long _cpuUsagePosition = -1;
        long _voiceInfoPosition = -1;
        long _bufferInfoPosition = -1;
        readonly long[] _gcPositions = new long[3];
        readonly bool[] _gcCollecteds = new bool[3];
        readonly Stopwatch _stopwatch = new Stopwatch();

        internal void BeginBuffer() => _stopwatch.Restart();
        internal void Pause() => _localStream.State = StreamState.Paused;
        internal void Resume() => _localStream.State = StreamState.Running;
        internal void OnGCNotification(int generation) => _gcCollecteds[generation] = true;

        internal void CopyStreamToUI(StreamModel streamUI)
        {
            if (streamUI == null) return;
            _localStream.CopyTo(streamUI);
            _localStream.IsClipping = _clip.Value;
            _localStream.IsExhausted = _exhausted.Value;
            _localStream.IsOverloaded = _overload.Value;
        }

        internal void Stop()
        {
            _stopwatch.Reset();
            _cpuUsageIndex = 0;
            _clip.Clear();
            _overload.Clear();
            _exhausted.Clear();
            _cpuUsagePosition = -1;
            _voiceInfoPosition = -1;
            _bufferInfoPosition = -1;
            _cpuUsageFactors = null;
            _cpuUsageFrameCounts = null;
            _cpuUsageTotalFrameCount = 0;
            new StreamModel(false).CopyTo(_localStream);
            for (int i = 0; i < _gcPositions.Length; i++)
            {
                _gcPositions[i] = -1;
                _gcCollecteds[i] = false;
            }
        }

        internal void Start(IAudioStream stream, in XtFormat format)
        {
            _cpuUsageIndex = 0;
            _cpuUsageTotalFrameCount = 0;
            _cpuUsageFactors = new double[format.mix.rate];
            _cpuUsageFrameCounts = new int[format.mix.rate];
            Native.SequencerOutput output;
            UpdateInfo(stream, &output, 0, format.mix.rate);
        }

        void UpdateFlags(int rate, long position)
        {
            _clip.Reset(rate, position);
            _overload.Reset(rate, position);
            _exhausted.Reset(rate, position);
            float infoFrames = InfoDurationSeconds * rate;
            if (position > _gcPositions[0] + infoFrames) _localStream.GC0Collected = false;
            if (position > _gcPositions[1] + infoFrames) _localStream.GC1Collected = false;
            if (position > _gcPositions[2] + infoFrames) _localStream.GC2Collected = false;
        }

        internal void EndBuffer(IAudioStream stream, in XtFormat format, Native.SequencerOutput* output, int frames)
        {
            UpdateFlags(format.mix.rate, output->position);
            _localStream.CurrentRow = output->row;
            _stopwatch.Stop();
            UpdateCpuUsage(frames, format.mix.rate, output->position);
            UpdateInfo(stream, output, frames, format.mix.rate);
        }

        internal void UpdateInfo(IAudioStream stream, Native.SequencerOutput* output, int frames, int rate)
        {
            float bufferSeconds = frames / (float)rate;
            var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
            _clip.Update(output->clip != 0, output->position);
            _exhausted.Update(output->exhausted != 0, output->position);
            _overload.Update(processedSeconds > bufferSeconds * OverloadLimit, output->position);
            if (_gcCollecteds[0])
            {
                _gcCollecteds[0] = false;
                _localStream.GC0Collected = true;
                _gcPositions[0] = output->position;
            }
            if (_gcCollecteds[1])
            {
                _gcCollecteds[1] = false;
                _localStream.GC1Collected = true;
                _gcPositions[1] = output->position;
            }
            if (_gcCollecteds[2])
            {
                _gcCollecteds[2] = false;
                _localStream.GC2Collected = true;
                _gcPositions[2] = output->position;
            }
            if (_bufferInfoPosition == -1 || output->position >=
                _bufferInfoPosition + rate * InfoDurationSeconds)
            {
                _bufferInfoPosition = output->position;
                _localStream.LatencyMs = stream.GetLatencyMs();
                _localStream.BufferSizeFrames = stream.GetMaxBufferFrames();
            }
            if (output->position >= _voiceInfoPosition + rate * InfoDurationSeconds)
            {
                _voiceInfoPosition = output->position;
                _localStream.Voices = output->voices;
            }
        }

        internal void UpdateCpuUsage(int frames, int rate, long position)
        {
            float bufferSeconds = frames / (float)rate;
            var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
            int cpuUsageCountToRemove = 0;
            _cpuUsageTotalFrameCount += frames;
            _cpuUsageFrameCounts[_cpuUsageIndex] = frames;
            _cpuUsageFactors[_cpuUsageIndex] = Math.Min(processedSeconds / bufferSeconds, 1.0);
            while (_cpuUsageTotalFrameCount > CpuUsageSamplingPeriodSeconds * rate)
                _cpuUsageTotalFrameCount -= _cpuUsageFrameCounts[_cpuUsageIndex - cpuUsageCountToRemove++];
            for (int i = 0; i <= _cpuUsageIndex - cpuUsageCountToRemove; i++)
            {
                _cpuUsageFactors[i] = _cpuUsageFactors[i + cpuUsageCountToRemove];
                _cpuUsageFrameCounts[i] = _cpuUsageFrameCounts[i + cpuUsageCountToRemove];
            }
            _cpuUsageIndex -= cpuUsageCountToRemove;
            double cpuUsage = 0.0;
            for (int i = 0; i <= _cpuUsageIndex; i++)
                cpuUsage += _cpuUsageFactors[i] * _cpuUsageFrameCounts[i] / _cpuUsageTotalFrameCount;
            _cpuUsageIndex++;
            if (position > _cpuUsagePosition + CpuUsageUpdateIntervalSeconds * rate)
            {
                _localStream.CpuUsage = cpuUsage;
                _cpuUsagePosition = position;
            }
        }
    }
}