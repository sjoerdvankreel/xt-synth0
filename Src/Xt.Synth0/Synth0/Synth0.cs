using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
    static class Synth0
    {
        static AudioEngine _engine;
        static unsafe Native.XtsPlot* _plot;
        static readonly AppModel Model = new AppModel();
        static readonly DateTime StartTime = DateTime.Now;

        const double AudioUpdateFps = 30.0;
        static long _lastAudioUpdateMs = 0;
        static readonly Stopwatch _audioUpdateTimer = new Stopwatch();

        [STAThread]
        static unsafe void Main()
        {
            try
            {
                _audioUpdateTimer.Start();
                StreamModel.MainThreadId = Thread.CurrentThread.ManagedThreadId;
                var infos = Model.Track.Synth.ParamInfos();
                fixed (SyncStepModel.Native* steps = SyncStepModel.Steps)
                    Native.XtsSyncStepModelInit(steps, SyncStepModel.Steps.Length);
                fixed (ParamInfo.Native* @params = infos)
                    Native.XtsSynthModelInit(@params, infos.Length);
                _plot = Native.XtsPlotCreate(SynthConfig.SynthParamCount);
                Model.Track.Synth.BindVoice(&_plot->model, &_plot->binding);
                Run();
            }
            finally
            {
                _engine?.Dispose();
                Native.XtsPlotDestroy(_plot);
            }
        }

        static void SaveAs(MainWindow window)
        => Save(window, LoadSaveUI.Save());
        static void Save(MainWindow window)
        => Save(window, window.Path ?? LoadSaveUI.Save());

        static void Run()
        {
            var app = new Application();
            app.Startup += OnAppStartup;
            PlotUI.RequestPlotData += OnRequestPlotData;
            PatternKeyUI.RequestPlayNote += OnRequestPlayNote;
            Model.Track.Synth.ParamChanged += OnSynthParamChanged;
            app.Dispatcher.Hooks.DispatcherInactive += OnDispatcherInactive;
            app.DispatcherUnhandledException += OnDispatcherUnhandledException;
            app.Run(CreateWindow());
        }

        static void OnSynthParamChanged(object sender, ParamChangedEventArgs e)
        {
            if (!Model.Stream.IsRunning) return;
            var value = Model.Track.Synth.Params[e.Target].Value;
            AutomationQueue.EnqueueUI(new AutomationAction.Native(e.Target, value));
        }

        static SettingsModel LoadSettings(AudioEngine engine)
        {
            var result = new SettingsModel();
            result.AsioDeviceId = engine.AsioDefaultDeviceId;
            result.WasapiDeviceId = engine.WasapiDefaultDeviceId;
            result.DSoundDeviceId = engine.DSoundDefaultDeviceId;
            try
            {
                return IO.LoadSettings() ?? result;
            }
            catch (Exception e)
            {
                OnError(e);
                return result;
            }
        }

        static void ShowSettings()
        {
            SettingsUI.Show(Model.Settings);
            IO.SaveSettings(Model.Settings);
            _engine.Reset();
        }

        static void New(MainWindow window)
        {
            if (!SaveUnsavedChanges(window)) return;
            new TrackModel().CopyTo(Model.Track);
            window.SetClean(null);
        }

        static void Load(MainWindow window)
        {
            if (!SaveUnsavedChanges(window)) return;
            Load(window, LoadSaveUI.Load());
        }

        static void LoadRecent(MainWindow window, string path)
        {
            if (!SaveUnsavedChanges(window)) return;
            Load(window, path);
        }

        static void Load(MainWindow window, string path)
        {
            if (path == null) return;
            try
            {
                PlotUI.BeginUpdate();
                IO.LoadFile(path).CopyTo(Model.Track);
            }
            finally
            {
                PlotUI.EndUpdate();
            }
            window.SetClean(path);
            Model.Settings.AddRecentFile(path);
            IO.SaveSettings(Model.Settings);
        }

        static void Save(MainWindow window, string path)
        {
            if (path == null) return;
            IO.SaveFile(Model.Track, path);
            window.SetClean(path);
            Model.Settings.AddRecentFile(path);
            IO.SaveSettings(Model.Settings);
        }

        static void OnDispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            OnError(e.Exception);
            e.Handled = true;
        }

        static void OnError(Exception error)
        {
            string message = error.Message;
            if (error is XtException xte)
                message = XtAudio.GetErrorInfo(xte.GetError()).ToString();
            IO.LogError(StartTime, message, error.StackTrace);
            var window = Application.Current.MainWindow;
            var showError = new Action(() => MessageBox.Show(window, message,
                "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            window.Dispatcher.BeginInvoke(showError);
        }

        static void OnDispatcherInactive(object sender, EventArgs e)
        {
            if (_audioUpdateTimer.ElapsedMilliseconds < _lastAudioUpdateMs + 1000.0 / AudioUpdateFps) return;
            var @params = Model.Track.Synth.Params;
            var queue = AutomationQueue.DequeueAudio();
            if (queue.Count > 0)
            {
                PlotUI.BeginUpdate();
                foreach (var action in queue)
                    @params[action.target].Value = action.value;
                PlotUI.EndUpdate();
            }
            _lastAudioUpdateMs = _audioUpdateTimer.ElapsedMilliseconds;
        }

        static bool SaveUnsavedChanges(MainWindow window)
        {
            if (!window.IsDirty) return true;
            var result = MessageBox.Show(window, "Save changes?",
                "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel) return false;
            if (result == MessageBoxResult.Yes) Save(window);
            return true;
        }

        static MainWindow CreateWindow()
        {
            var result = new MainWindow(Model);
            BindCommand(result, ApplicationCommands.New, () => New(result));
            BindCommand(result, ApplicationCommands.Open, () => Load(result));
            BindCommand(result, ApplicationCommands.Save, () => Save(result));
            BindCommand(result, ApplicationCommands.SaveAs, () => SaveAs(result));
            result.Closing += (s, e) => e.Cancel = !SaveUnsavedChanges(result);
            return result;
        }

        static void BindCommand(Window window, RoutedUICommand command, Action handler)
        {
            if (command.InputGestures.Count > 0)
                window.InputBindings.Add(new InputBinding(command, command.InputGestures[0]));
            window.CommandBindings.Add(new CommandBinding(command, (s, e) => handler()));
        }

        static void OnAppStartup(object sender, StartupEventArgs e)
        {
            var window = (MainWindow)Application.Current.MainWindow;
            _engine = SetupEngine(window);
            LoadSettings(_engine).CopyTo(Model.Settings);
            MenuUI.New += (s, e) => New(window);
            MenuUI.Open += (s, e) => Load(window);
            MenuUI.Save += (s, e) => Save(window);
            MenuUI.SaveAs += (s, e) => SaveAs(window);
            MenuUI.ShowSettings += (s, e) => ShowSettings();
            MenuUI.OpenRecent += (s, e) => LoadRecent(window, e.Path);
            ControlUI.Stop += (s, e) => _engine.Stop(true);
            ControlUI.Start += (s, e) => _engine.Start(Model.Track.Seq, Model.Stream);
            Action showPanel = () => _engine.ShowASIOControlPanel(Model.Settings.AsioDeviceId);
            SettingsUI.QueryFormatSupport += OnQueryFormatSupport;
            SettingsUI.ShowASIOControlPanel += (s, e) => showPanel();
        }

        static void OnQueryFormatSupport(object sender, QueryFormatSupportEventArgs e)
        {
            var support = _engine.QueryFormatSupport();
            e.IsSupported = support != null;
            e.MinBuffer = support?.min ?? 0.0;
            e.MaxBuffer = support?.max ?? 0.0;
            e.DefaultBuffer = support?.current ?? 0.0;
        }

        static void OnRequestPlayNote(object sender, RequestPlayNoteEventArgs e)
        {
            var seq = new SequencerModel();
            seq.Edit.Loop.Value = 0;
            seq.Edit.Rows.Value = 1;
            seq.Pattern.Rows[0].Keys[0].Octave.Value = e.Oct;
            seq.Pattern.Rows[0].Keys[0].Note.Value = (int)e.Note;
            _engine.Stop(false);
            _engine.Start(seq, new StreamModel(false));
        }

        static unsafe void OnRequestPlotData(object sender, RequestPlotDataEventArgs e)
        {
            Native.PlotInput input;
            PlotOutput.Native* output;
            int rate = Model.Settings.SampleRate.ToInt();
            if (rate == 0 || e.Pixels == 0) return;
            input.rate = rate;
            input.pixels = e.Pixels;
            input.bpm = Model.Track.Seq.Edit.Bpm.Value;
            input.spectrum = Model.Track.Synth.Plot.Spectrum.Value == 0 ? 0 : 1;
            Model.Track.Synth.ToNative(&_plot->binding);
            e.Result = Native.XtsPlotRender(_plot, &input, &output);
            e.Output = output;
        }

        static AudioEngine SetupEngine(Window mainWindow)
        {
            var helper = new WindowInteropHelper(mainWindow);
            var logger = (string msg) => IO.LogError(StartTime, msg, null);
            Action<Action> dispatchToUI = a => Application.Current?.Dispatcher.BeginInvoke(a);
            var result = AudioEngine.Create(helper.Handle, Model.Settings, Model.Track.Synth, logger, dispatchToUI);
            AudioIOModel.AddAsioDevices(result.AsioDevices);
            AudioIOModel.AddWasapiDevices(result.WasapiDevices);
            AudioIOModel.AddDSoundDevices(result.DSoundDevices);
            return result;
        }
    }
}