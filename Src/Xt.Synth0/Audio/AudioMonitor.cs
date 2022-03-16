using System;
using System.Diagnostics;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
    unsafe class AudioMonitor
    {
        const float OverloadLimit = 0.9f;
        const float InfoDurationSeconds = 0.5f;
        const float CpuUsageUpdateIntervalSeconds = 1.0f;
        const float CpuUsageSamplingPeriodSeconds = 0.5f;

        int _cpuUsageIndex;
        double[] _cpuUsageFactors;
        int[] _cpuUsageFrameCounts;
        int _cpuUsageTotalFrameCount;
        readonly StreamModel _localStream = new(false);

        long _clipPosition = -1;
        long _cpuUsagePosition = -1;
        long _overloadPosition = -1;
        long _voiceInfoPosition = -1;
        long _exhaustedPosition = -1;
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
            if (streamUI != null)
                _localStream.CopyTo(streamUI);
        }

        internal void Stop()
        {
            _stopwatch.Reset();
            _cpuUsageIndex = 0;
            _clipPosition = -1;
            _overloadPosition = -1;
            _cpuUsagePosition = -1;
            _voiceInfoPosition = -1;
            _exhaustedPosition = -1;
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
            UpdateInfo(stream, 0, format.mix.rate, 0, false, false, 0);
        }

        void ResetWarnings(int rate, long streamPosition)
        {
            float infoFrames = InfoDurationSeconds * rate;
            if (streamPosition > _clipPosition + infoFrames) _localStream.IsClipping = false;
            if (streamPosition > _gcPositions[0] + infoFrames) _localStream.GC0Collected = false;
            if (streamPosition > _gcPositions[1] + infoFrames) _localStream.GC1Collected = false;
            if (streamPosition > _gcPositions[2] + infoFrames) _localStream.GC2Collected = false;
            if (streamPosition > _overloadPosition + infoFrames) _localStream.IsOverloaded = false;
            if (streamPosition > _exhaustedPosition + infoFrames) _localStream.IsExhausted = false;
        }

        internal void EndBuffer(IAudioStream stream, in XtFormat format, Native.SequencerOutput* output, int frames)
        {
            ResetWarnings(format.mix.rate, output->position);
            _localStream.CurrentRow = output->row;
            _stopwatch.Stop();
            UpdateCpuUsage(frames, format.mix.rate, output->position);
            bool clip = output->clip != 0;
            bool exhausted = output->exhausted != 0;
            UpdateInfo(stream, frames, format.mix.rate, output->voices, clip, exhausted, output->position);
        }

        internal void UpdateInfo(IAudioStream stream, int frames, int rate, int voices, bool clip, bool exhausted, long position)
        {
            float bufferSeconds = frames / (float)rate;
            var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
            if (clip)
            {
                _localStream.IsClipping = true;
                _clipPosition = position;
            }
            if (exhausted)
            {
                _localStream.IsExhausted = true;
                _exhaustedPosition = position;
            }
            if (_gcCollecteds[0])
            {
                _gcCollecteds[0] = false;
                _localStream.GC0Collected = true;
                _gcPositions[0] = position;
            }
            if (_gcCollecteds[1])
            {
                _gcCollecteds[1] = false;
                _localStream.GC1Collected = true;
                _gcPositions[1] = position;
            }
            if (_gcCollecteds[2])
            {
                _gcCollecteds[2] = false;
                _localStream.GC2Collected = true;
                _gcPositions[2] = position;
            }
            if (processedSeconds > bufferSeconds * OverloadLimit)
            {
                _overloadPosition = position;
                _localStream.IsOverloaded = true;
            }
            if (_bufferInfoPosition == -1 || position >=
                _bufferInfoPosition + rate * InfoDurationSeconds)
            {
                _bufferInfoPosition = position;
                _localStream.LatencyMs = stream.GetLatencyMs();
                _localStream.BufferSizeFrames = stream.GetMaxBufferFrames();
            }
            if (position >= _voiceInfoPosition + rate * InfoDurationSeconds)
            {
                _voiceInfoPosition = position;
                _localStream.Voices = voices;
            }
        }

        internal void UpdateCpuUsage(int frames, int rate, long streamPosition)
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
            if (streamPosition > _cpuUsagePosition + CpuUsageUpdateIntervalSeconds * rate)
            {
                _localStream.CpuUsage = cpuUsage;
                _cpuUsagePosition = streamPosition;
            }
        }
    }
}