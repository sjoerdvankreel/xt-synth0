using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xt.Synth0.Model;

namespace Xt.Synth0
{
    unsafe class AudioEngine : IDisposable
    {
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
        readonly int[] _automationValues;

        readonly Action _stopStream;
        readonly XtPlatform _platform;
        readonly Action _copyStreamToUI;
        readonly Action<Action> _dispatchToUI;

        readonly SynthModel _synth;
        readonly SettingsModel _settings;
        readonly AudioMonitor _monitor = new();
        readonly SynthModel _localSynth = new();
        readonly SynthModel _originalSynth = new();

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
            GCNotification.Register(_monitor);

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

        void CopyStreamToUI() => _monitor.CopyStreamToUI(_streamUI);
        void OnXtRunning(XtStream stream, bool running, ulong error, object user) => OnRunning(running, error);

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
                _monitor.Pause();
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
                _synth.ToNative(_sequencer->binding);
                _streamUI.State = StreamState.Running;
                _audioStream.Start();
                _monitor.Resume();
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
            _monitor.Stop();
            new StreamModel(false).CopyTo(_streamUI);
            _streamUI = null;
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

        XtDevice OpenDevice(XtSystem system, string deviceId, string defaultId)
        {
            var service = _platform.GetService(system);
            var id = string.IsNullOrEmpty(deviceId) ? defaultId : deviceId;
            return service.OpenDevice(id);
        }

        internal void ShowASIOControlPanel(string deviceId)
        {
            using var device = OpenDevice(XtSystem.ASIO, deviceId, AsioDefaultDeviceId);
            device.ShowControlPanel();
        }

        void BeginAutomation(AutomationAction.Native* actions, int count)
        {
            var @params = _localSynth.Params;
            for (int i = 0; i < count; i++)
                @params[actions[i].target].Value = actions[i].value;
            for (int i = 0; i < @params.Count; i++)
                _automationValues[i] = @params[i].Value;
        }

        void EndAutomation()
        {
            _localSynth.FromNative(_sequencer->binding);
            var @params = _localSynth.Params;
            for (int i = 0; i < @params.Count; i++)
                if (@params[i].Value != _automationValues[i])
                    AutomationQueue.EnqueueAudio(new AutomationAction.Native(i, @params[i].Value));
        }

        internal unsafe void OnBuffer(in XtBuffer buffer, in XtFormat format)
        {
            _monitor.BeginBuffer();
            var actions = AutomationQueue.DequeueUI(out int count);
            BeginAutomation(actions, count);
            var output = Native.XtsSequencerRender(_sequencer, buffer.frames, actions, count);
            EndAutomation();
            BufferConvert.To(output->buffer, buffer.output, format.mix.sample, buffer.frames);
            _monitor.EndBuffer(_audioStream, in format, output, buffer.frames);
            if (output->end != 0) _dispatchToUI(_stopStream);
            else _dispatchToUI(_copyStreamToUI);
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

        void StartStream(SequencerModel seq)
        {
            try
            {
                var edit = seq.Edit;
                var format = GetFormat();
                var bufferSize = _settings.BufferSize.ToInt();
                var streamParams = new XtStreamParams(true, OnXtBuffer, null, OnXtRunning);
                var deviceParams = new XtDeviceStreamParams(in streamParams, in format, bufferSize);
                if (_audioStream == null)
                    if (_settings.WriteToDisk)
                        _audioStream = new DiskStream(this, in format, bufferSize, _settings.OutputPath);
                    else
                        _audioStream = OpenDeviceStream(in deviceParams);
                int frames = _audioStream.GetMaxBufferFrames();
                _sequencer = Native.XtsSequencerCreate(SynthConfig.SynthParamCount, frames, edit.Fxs.Value, edit.Keys.Value, edit.Bpm.Value, format.mix.rate);
                seq.ToNative(_sequencer->sequencerModel);
                _synth.Bind(_sequencer->synthModel, _sequencer->binding);
                _synth.ToNative(_sequencer->binding);
                Native.XtsSequencerInit(_sequencer);
                _monitor.Start(_audioStream, in format);
                ResumeStream();
            }
            catch
            {
                StopStream();
                throw;
            }
        }
    }
}