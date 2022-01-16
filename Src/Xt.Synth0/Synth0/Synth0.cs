using System;
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
		static IntPtr _nativePlotDSP;
		static IntPtr _nativePlotSynthModel;
		static unsafe Native.PlotInput* _nativePlotInput;
		static unsafe Native.PlotOutput* _nativePlotOutput;

		static AudioEngine _engine;
		static readonly AppModel Model = new AppModel();
		static readonly DateTime StartTime = DateTime.Now;

		[STAThread]
		static unsafe void Main()
		{
			TrackConstants.SanityChecks();
			try
			{
				_nativePlotDSP = Native.XtsPlotDSPCreate();
				_nativePlotInput = Native.XtsPlotInputCreate();
				_nativePlotOutput = Native.XtsPlotOutputCreate();
				_nativePlotSynthModel = Native.XtsSynthModelCreate();
				Run();
			}
			finally
			{
				_engine?.Dispose();
				Native.XtsPlotDSPDestroy(_nativePlotDSP);
				Native.XtsPlotInputDestroy(_nativePlotInput);
				Native.XtsPlotOutputDestroy(_nativePlotOutput);
				Native.XtsSynthModelDestroy(_nativePlotSynthModel);
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
			app.Dispatcher.Hooks.DispatcherInactive += OnDispatcherInactive;
			app.DispatcherUnhandledException += OnDispatcherUnhandledException;
			Model.Track.Synth.ParamChanged += OnSynthParamChanged;
			app.Run(CreateWindow());
		}

		static void OnSynthParamChanged(object sender, ParamChangedEventArgs e)
		{
			if (!Model.Stream.IsRunning) return;
			var value = Model.Track.Synth.Params[e.Index].Value;
			AutomationQueue.EnqueueUI(e.Index, value);
		}

		static SettingsModel LoadSettings(AudioEngine engine)
		{
			var result = new SettingsModel();
			result.AsioDeviceId = engine.AsioDefaultDeviceId;
			result.WasapiDeviceId = engine.WasapiDefaultDeviceId;
			try
			{
				IO.LoadSetting(result);
			}
			catch (Exception e)
			{
				OnError(e);
			}
			return result;
		}

		static void ShowSettings()
		{
			SettingsUI.Show(Model.Settings);
			IO.SaveSettings(Model.Settings);
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
			IO.LoadFile(path, Model.Track);
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

		static void OnDispatcherInactive(object sender, EventArgs e)
		{
			var @params = Model.Track.Synth.Params;
			var actions = AutomationQueue.DequeueAudio(out var count);
			for (int i = 0; i < count; i++)
				@params[actions[i].Param].Value = @actions[i].Value;
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
			ControlUI.Stop += (s, e) => _engine.Stop();
			ControlUI.Start += (s, e) => _engine.Start();
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

		static AudioEngine SetupEngine(Window mainWindow)
		{
			var helper = new WindowInteropHelper(mainWindow);
			var logger = (string msg) => IO.LogError(StartTime, msg, null);
			Action<Action> dispatchToUI = a => Application.Current?.Dispatcher.BeginInvoke(a);
			var result = AudioEngine.Create(Model, helper.Handle, logger, dispatchToUI);
			AudioModel.AddAsioDevices(result.AsioDevices);
			AudioModel.AddWasapiDevices(result.WasapiDevices);
			return result;
		}

		static unsafe void OnRequestPlotData(object sender, RequestPlotDataEventArgs e)
		{
			_nativePlotInput->pixels = e.Pixels;
			_nativePlotInput->synth = _nativePlotSynthModel;
			_nativePlotInput->rate = Model.Settings.SampleRate.ToInt();
			Model.Track.Synth.ToNative(_nativePlotSynthModel.ToPointer());
			Native.XtsPlotDSPRender(_nativePlotDSP, _nativePlotInput, _nativePlotOutput);

			e.Freq = _nativePlotOutput->freq;
			e.Clip = _nativePlotOutput->clip != 0;
			e.SampleRate = _nativePlotOutput->rate;
			e.Bipolar = _nativePlotOutput->bipolar != 0;
			e.Splits.Clear();
			for (int i = 0; i < _nativePlotOutput->splitCount; i++)
				e.Splits.Add(_nativePlotOutput->splits[i]);
			e.Samples.Clear();
			for (int i = 0; i < _nativePlotOutput->sampleCount; i++)
				e.Samples.Add(_nativePlotOutput->samples[i]);
		}
	}
}