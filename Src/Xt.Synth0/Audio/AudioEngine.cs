using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
    unsafe class AudioEngine : IDisposable
    {
        const float OverloadLimit = 0.9f;
        const float InfoDurationSeconds = 0.5f;
        const float CpuUsageUpdateIntervalSeconds = 1.0f;
        const float CpuUsageSamplingPeriodSeconds = 0.5f;

        static XtSample DepthToSample(int size) => size switch
        {
            16 => XtSample.Int16,
            24 => XtSample.Int24,
            32 => XtSample.Int32,
            _ => throw new InvalidOperationException()
        };

        internal static AudioEngine Create(IntPtr mainWindow, SettingsModel settings,
            SynthModel synth, Action<string> log, Action<Action> dispatchToUI)
        {
            XtAudio.SetOnError(msg => log(msg));
            var platform = XtAudio.Init(nameof(Synth0), mainWindow);
            try
            {
                return Create(platform, settings, synth, dispatchToUI);
            }
            catch
            {
                platform.Dispose();
                throw;
            }
        }

        static AudioEngine Create(XtPlatform platform, SettingsModel settings,
            SynthModel synth, Action<Action> dispatchToUI)
        {
            var asio = platform.GetService(XtSystem.ASIO);
            var wasapi = platform.GetService(XtSystem.WASAPI);
            var dSound = platform.GetService(XtSystem.DirectSound);
            return new AudioEngine(platform, settings, synth, dispatchToUI,
                asio.GetDefaultDeviceId(true),
                wasapi.GetDefaultDeviceId(true),
                dSound.GetDefaultDeviceId(true),
                GetDevices(asio), GetDevices(wasapi), GetDevices(dSound));
        }

        static IReadOnlyList<DeviceModel> GetDevices(XtService service)
        {
            var result = new List<DeviceModel>();
            using var list = service.OpenDeviceList(XtEnumFlags.Output);
            for (int d = 0; d < list.GetCount(); d++)
            {
                var id = list.GetId(d);
                result.Add(new DeviceModel(id, list.GetName(id)));
            }
            return new ReadOnlyCollection<DeviceModel>(result);
        }

        StreamModel _streamUI;
        IAudioStream _audioStream;
        Native.XtsSequencer* _sequencer;

        int _cpuUsageIndex;
        double[] _cpuUsageFactors;
        int[] _cpuUsageFrameCounts;
        int _cpuUsageTotalFrameCount;
        readonly int[] _automationValues;

        readonly Action _stopStream;
        readonly XtPlatform _platform;
        readonly Action _copyStreamToUI;
        readonly Action<Action> _dispatchToUI;

        readonly SynthModel _synth;
        readonly SettingsModel _settings;
        readonly SynthModel _localSynth = new();
        readonly SynthModel _originalSynth = new();
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

        public string AsioDefaultDeviceId { get; }
        public string WasapiDefaultDeviceId { get; }
        public string DSoundDefaultDeviceId { get; }
        public IReadOnlyList<DeviceModel> AsioDevices { get; }
        public IReadOnlyList<DeviceModel> WasapiDevices { get; }
        public IReadOnlyList<DeviceModel> DSoundDevices { get; }

        AudioEngine(
            XtPlatform platform,
            SettingsModel settings,
            SynthModel synth,
            Action<Action> dispatchToUI,
            string asioDefaultDeviceId,
            string wasapiDefaultDeviceId,
            string dSoundDefaultDeviceId,
            IReadOnlyList<DeviceModel> asioDevices,
            IReadOnlyList<DeviceModel> wasapiDevices,
            IReadOnlyList<DeviceModel> dSoundDevices)
        {
            GCNotification.Register(this);

            AsioDevices = asioDevices;
            WasapiDevices = wasapiDevices;
            DSoundDevices = dSoundDevices;
            AsioDefaultDeviceId = asioDefaultDeviceId;
            WasapiDefaultDeviceId = wasapiDefaultDeviceId;
            DSoundDefaultDeviceId = dSoundDefaultDeviceId;

            _synth = synth;
            _settings = settings;
            _platform = platform;
            _stopStream = StopStream;
            _dispatchToUI = dispatchToUI;
            _copyStreamToUI = CopyStreamToUI;
            _automationValues = new int[_originalSynth.Params.Count];
        }

        internal void OnGCNotification(int generation) => _gcCollecteds[generation] = true;

        void CopyStreamToUI()
        {
            var streamUI = _streamUI;
            if (streamUI != null)
                _localStream.CopyTo(streamUI);
        }

        public void Dispose()
        {
            Reset();
            _platform.Dispose();
        }

        internal void Reset()
        {
            StopStream();
            _audioStream?.Dispose();
            _audioStream = null;
        }

        internal void Stop(bool pause)
        {
            if (pause && _streamUI?.IsRunning == true)
                PauseStream();
            else
                StopStream();
        }

        internal void Start(SequencerModel seq, StreamModel stream)
        {
            if (_streamUI != null && _streamUI != stream)
                StopStream();
            _streamUI = stream;
            if (_streamUI.IsPaused)
                ResumeStream();
            else
                StartStream(seq);
        }

        void PauseStream()
        {
            try
            {
                _audioStream?.Stop();
                _originalSynth.CopyTo(_synth);
                _streamUI.State = StreamState.Paused;
                _localStream.State = StreamState.Paused;
            }
            catch
            {
                StopStream();
                throw;
            }
        }

        void ResumeStream()
        {
            try
            {
                AutomationQueue.Clear();
                _synth.CopyTo(_localSynth);
                _synth.CopyTo(_originalSynth);
                _synth.ToNative(&_sequencer->binding);
                _streamUI.State = StreamState.Running;
                _localStream.State = StreamState.Running;
                _audioStream.Start();
            }
            catch
            {
                StopStream();
                throw;
            }
        }

        void StopStream()
        {
            if (_streamUI == null) return;

            PauseStream();

            Native.XtsSequencerDestroy(_sequencer);
            _sequencer = null;
            _stopwatch.Reset();

            _clipPosition = -1;
            _overloadPosition = -1;
            _cpuUsagePosition = -1;
            _voiceInfoPosition = -1;
            _exhaustedPosition = -1;
            _bufferInfoPosition = -1;

            _cpuUsageIndex = 0;
            _cpuUsageFactors = null;
            _cpuUsageFrameCounts = null;
            _cpuUsageTotalFrameCount = 0;
            new StreamModel(false).CopyTo(_streamUI);
            new StreamModel(false).CopyTo(_localStream);
            _streamUI = null;

            for (int i = 0; i < _gcPositions.Length; i++)
            {
                _gcPositions[i] = -1;
                _gcCollecteds[i] = false;
            }
        }

        void StartStream(SequencerModel seq)
        {
            try
            {
                var format = GetFormat();
                var bufferSize = _settings.BufferSize.ToInt();
                var streamParams = new XtStreamParams(true, OnXtBuffer, null, OnXtRunning);
                var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
                if (_audioStream == null)
                    if (_settings.WriteToDisk)
                        _audioStream = new DiskStream(this, in format, bufferSize, _settings.OutputPath);
                    else
                        _audioStream = OpenDeviceStream(in deviceParams);
                _cpuUsageIndex = 0;
                _cpuUsageTotalFrameCount = 0;
                _cpuUsageFactors = new double[format.mix.rate];
                _cpuUsageFrameCounts = new int[format.mix.rate];
                UpdateStreamInfo(0, format.mix.rate, 0, false, false, 0);
                _sequencer = Native.XtsSequencerCreate(SynthConfig.ParamCount, _audioStream.GetMaxBufferFrames(), format.mix.rate);
                seq.ToNative(&_sequencer->model);
                _synth.BindVoice(&_sequencer->synth, &_sequencer->binding);
                ResumeStream();
            }
            catch
            {
                StopStream();
                throw;
            }
        }

        void ResetWarnings(int rate, long streamPosition)
        {
            float infoFrames = InfoDurationSeconds * rate;
            if (streamPosition > _clipPosition + infoFrames)
                _localStream.IsClipping = false;
            if (streamPosition > _gcPositions[0] + infoFrames)
                _localStream.GC0Collected = false;
            if (streamPosition > _gcPositions[1] + infoFrames)
                _localStream.GC1Collected = false;
            if (streamPosition > _gcPositions[2] + infoFrames)
                _localStream.GC2Collected = false;
            if (streamPosition > _overloadPosition + infoFrames)
                _localStream.IsOverloaded = false;
            if (streamPosition > _exhaustedPosition + infoFrames)
                _localStream.IsExhausted = false;
        }

        void UpdateStreamInfo(int frames, int rate, int voices, bool clip, bool exhausted, long streamPosition)
        {
            float bufferSeconds = frames / (float)rate;
            var processedSeconds = _stopwatch.Elapsed.TotalSeconds;
            if (clip)
            {
                _localStream.IsClipping = true;
                _clipPosition = streamPosition;
            }
            if (exhausted)
            {
                _localStream.IsExhausted = true;
                _exhaustedPosition = streamPosition;
            }
            if (_gcCollecteds[0])
            {
                _gcCollecteds[0] = false;
                _localStream.GC0Collected = true;
                _gcPositions[0] = streamPosition;
            }
            if (_gcCollecteds[1])
            {
                _gcCollecteds[1] = false;
                _localStream.GC1Collected = true;
                _gcPositions[1] = streamPosition;
            }
            if (_gcCollecteds[2])
            {
                _gcCollecteds[2] = false;
                _localStream.GC2Collected = true;
                _gcPositions[2] = streamPosition;
            }
            if (processedSeconds > bufferSeconds * OverloadLimit)
            {
                _overloadPosition = streamPosition;
                _localStream.IsOverloaded = true;
            }
            if (_bufferInfoPosition == -1 || streamPosition >=
                _bufferInfoPosition + rate * InfoDurationSeconds)
            {
                _bufferInfoPosition = streamPosition;
                _localStream.LatencyMs = _audioStream.GetLatencyMs();
                _localStream.BufferSizeFrames = _audioStream.GetMaxBufferFrames();
            }
            if (streamPosition >= _voiceInfoPosition + rate * InfoDurationSeconds)
            {
                _voiceInfoPosition = streamPosition;
                _localStream.Voices = voices;
            }
        }

        void UpdateCpuUsage(int frames, int rate, long streamPosition)
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

        void CopyBuffer(in XtBuffer buffer, in XtFormat format, float* output)
        {
            switch (format.mix.sample)
            {
                case XtSample.Int16: CopyBuffer16(buffer, output); break;
                case XtSample.Int24: CopyBuffer24(buffer, output); break;
                case XtSample.Int32: CopyBuffer32(buffer, output); break;
                default: throw new InvalidOperationException();
            }
        }

        unsafe void CopyBuffer32(in XtBuffer buffer, float* output)
        {
            int* samples = (int*)buffer.output;
            for (int f = 0; f < buffer.frames; f++)
            {
                samples[f * 2] = (int)(output[f * 2] * int.MaxValue);
                samples[f * 2 + 1] = (int)(output[f * 2 + 1] * int.MaxValue);
            }
        }

        unsafe void CopyBuffer16(in XtBuffer buffer, float* output)
        {
            short* samples = (short*)buffer.output;
            for (int f = 0; f < buffer.frames; f++)
            {
                samples[f * 2] = (short)(output[f * 2] * short.MaxValue);
                samples[f * 2 + 1] = (short)(output[f * 2 + 1] * short.MaxValue);
            }
        }

        unsafe void CopyBuffer24(in XtBuffer buffer, float* output)
        {
            byte* bytes = (byte*)buffer.output;
            for (int f = 0; f < buffer.frames; f++)
            {
                int left = (int)(output[f * 2] * int.MaxValue);
                int right = (int)(output[f * 2 + 1] * int.MaxValue);
                bytes[f * 6 + 0] = (byte)((left & 0x0000FF00) >> 8);
                bytes[f * 6 + 1] = (byte)((left & 0x00FF0000) >> 16);
                bytes[f * 6 + 2] = (byte)((left & 0xFF000000) >> 24);
                bytes[f * 6 + 3] = (byte)((right & 0x0000FF00) >> 8);
                bytes[f * 6 + 4] = (byte)((right & 0x00FF0000) >> 16);
                bytes[f * 6 + 5] = (byte)((right & 0xFF000000) >> 24);
            }
        }

        void BeginAutomation()
        {
            var @params = _localSynth.Params;
            foreach (var action in AutomationQueue.DequeueUI())
                @params[action.Param].Value = action.Value;
            for (int i = 0; i < @params.Count; i++)
                _automationValues[i] = @params[i].Value;
            _localSynth.ToNative(&_sequencer->binding);
        }

        void EndAutomation()
        {
            _localSynth.FromNative(&_sequencer->binding);
            var @params = _localSynth.Params;
            for (int i = 0; i < @params.Count; i++)
                if (@params[i].Value != _automationValues[i])
                    AutomationQueue.EnqueueAudio(i, @params[i].Value);
        }

        internal unsafe void OnBuffer(in XtBuffer buffer, in XtFormat format)
        {
            _stopwatch.Restart();
            BeginAutomation();
            var output = Native.XtsSequencerRender(_sequencer, buffer.frames);
            EndAutomation();
            CopyBuffer(in buffer, in format, output->buffer);
            ResetWarnings(format.mix.rate, output->position);
            _localStream.CurrentRow = output->row;
            _stopwatch.Stop();
            UpdateCpuUsage(buffer.frames, format.mix.rate, output->position);
            bool clip = output->clip != 0;
            bool exhausted = output->exhausted != 0;
            UpdateStreamInfo(buffer.frames, format.mix.rate, output->voices, clip, exhausted, output->position);
            if (output->end != 0) _dispatchToUI(_stopStream);
            else _dispatchToUI(_copyStreamToUI);
        }

        internal void OnRunning(bool running, ulong error)
        {
            if (!running && error != 0)
                _dispatchToUI(StopStream);
        }

        int OnXtBuffer(XtStream stream, in XtBuffer buffer, object user)
        {
            OnBuffer(buffer, stream.GetFormat());
            return 0;
        }

        void OnXtRunning(XtStream stream, bool running, ulong error, object user)
        {
            OnRunning(running, error);
        }

        internal XtBufferSize? QueryFormatSupport()
        {
            var format = GetFormat();
            using var device = OpenDevice();
            if (!device.SupportsFormat(in format)) return null;
            return device.GetBufferSize(in format);
        }

        XtFormat GetFormat()
        {
            var depth = _settings.BitDepth.ToInt();
            var rate = _settings.SampleRate.ToInt();
            var sample = DepthToSample(depth);
            var mix = new XtMix(rate, sample);
            var channels = new XtChannels(0, 0, 2, 0);
            return new XtFormat(in mix, in channels);
        }

        XtDevice OpenDevice(XtSystem system, string deviceId, string defaultId)
        {
            var service = _platform.GetService(system);
            var id = string.IsNullOrEmpty(deviceId) ? defaultId : deviceId;
            return service.OpenDevice(id);
        }

        IAudioStream OpenDeviceStream(in XtDeviceStreamParams deviceParams)
        {
            XtDevice device = OpenDevice();
            try
            {
                var stream = device.OpenStream(in deviceParams, null);
                return new DeviceStream(device, stream);
            }
            catch
            {
                device.Dispose();
                throw;
            }
        }

        internal void ShowASIOControlPanel(string deviceId)
        {
            using var device = OpenDevice(XtSystem.ASIO, deviceId, AsioDefaultDeviceId);
            device.ShowControlPanel();
        }

        XtDevice OpenDevice()
        {
            XtSystem system;
            string defaultId;
            string selectedId;
            switch (_settings.DeviceType)
            {
                case DeviceType.Asio:
                    system = XtSystem.ASIO;
                    defaultId = AsioDefaultDeviceId;
                    selectedId = _settings.AsioDeviceId;
                    break;
                case DeviceType.Wasapi:
                    system = XtSystem.WASAPI;
                    defaultId = WasapiDefaultDeviceId;
                    selectedId = _settings.WasapiDeviceId;
                    break;
                case DeviceType.DSound:
                    system = XtSystem.DirectSound;
                    defaultId = DSoundDefaultDeviceId;
                    selectedId = _settings.DSoundDeviceId;
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return OpenDevice(system, selectedId, defaultId);
        }
    }
}