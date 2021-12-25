using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Xt.Synth0.DSP;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	static class Synth0
	{
		const int PlotCycles = 2;
		const int PlotBufferSize = 192000;
		static readonly float[] PlotBuffer = new float[PlotBufferSize];

		static AudioEngine _engine;
		static readonly AppModel Model = new AppModel();
		static readonly DateTime StartTime = DateTime.Now;

		static readonly ParamAction[] _uiThreadActions
		= new ParamAction[Model.Synth.Params().Count];

		[STAThread]
		static void Main()
		{
			try
			{
				Run();
			}
			finally
			{
				_engine?.Dispose();
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
			app.DispatcherUnhandledException += OnDispatcherUnhandledException;
			app.Run(CreateWindow());
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
			new SynthModel().CopyTo(Model.Synth, false);
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
			IO.LoadFile(path, Model.Synth);
			window.SetClean(path);
			Model.Settings.AddRecentFile(path);
			IO.SaveSettings(Model.Settings);
		}

		static void Save(MainWindow window, string path)
		{
			if (path == null) return;
			IO.SaveFile(Model.Synth, path);
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
			ControlUI.Stop += OnStop;
			ControlUI.Start += OnStart;
			MenuUI.New += (s, e) => New(window);
			MenuUI.Open += (s, e) => Load(window);
			MenuUI.Save += (s, e) => Save(window);
			MenuUI.SaveAs += (s, e) => SaveAs(window);
			MenuUI.ShowSettings += (s, e) => ShowSettings();
			MenuUI.OpenRecent += (s, e) => LoadRecent(window, e.Path);
			Action showPanel = () => _engine.ShowASIOControlPanel(Model.Settings.AsioDeviceId);
			SettingsUI.ShowASIOControlPanel += (s, e) => showPanel();
			Model.Synth.ParamChanged += OnSynthParamChanged;
		}

		static void OnStop(object sender, EventArgs e)
		{
			_engine.Stop();
			Array.Clear(_uiThreadActions);
		}

		static void OnStart(object sender, EventArgs e)
		{
			Array.Clear(_uiThreadActions);
			_engine.Start();
		}

		static void CopyToUIThread(SynthModel model)
		{
			if (Model.Audio.IsRunning)
			{
				for (int u = 0; u < _uiThreadActions.Length; u++)
					if (_uiThreadActions[u].Changed)
						model.Params()[u].Value = _uiThreadActions[u].Value;
				model.CopyTo(Model.Synth, true);
			}
			ModelPool.Return(model);
			Array.Clear(_uiThreadActions);
		}

		static void OnSynthParamChanged(object sender, ParamChangedEventArgs e)
		{
			if (!Model.Audio.IsRunning) return;
			_uiThreadActions[e.Index].Changed = true;
			_uiThreadActions[e.Index].Value = e.Value;
		}

		static void OnBufferFinished(SynthModel model, Action<SynthModel> copyToUIThread)
		{
			var dispatcher = Application.Current?.Dispatcher;
			if (dispatcher == null) return;
			if (ModelPool.CurrentCopyOperation?.Abort() == true)
				ModelPool.Return(ModelPool.CurrentModel);
			ModelPool.CurrentModel = model;
			ModelPool.CurrentCopyOperation = dispatcher.BeginInvoke(copyToUIThread, model);
		}

		static AudioEngine SetupEngine(Window mainWindow)
		{
			var helper = new WindowInteropHelper(mainWindow);
			Action<SynthModel> copyToUIThread = CopyToUIThread;
			var logger = (string msg) => IO.LogError(StartTime, msg, null);
			Action<SynthModel> bufferFinished = m => OnBufferFinished(m, copyToUIThread);
			Action<Action> dispatchToUI = a => Application.Current?.Dispatcher.BeginInvoke(a);
			var result = AudioEngine.Create(Model, helper.Handle, logger, dispatchToUI, bufferFinished);
			AudioModel.AddAsioDevices(result.AsioDevices);
			AudioModel.AddWasapiDevices(result.WasapiDevices);
			return result;
		}

		static void OnRequestPlotData(object sender, RequestPlotDataEventArgs e)
		{
			e.Data = PlotBuffer;
			var dsp = new UnitDSP();
			var synth = Model.Synth;
			var global = synth.Global;
			var index = global.Plot.Value;
			var unit = synth.Units[index - 1];
			e.Frequency = dsp.Frequency(unit);
			var rate = AudioModel.RateToInt(Model.Settings.SampleRate);
			var cycleLength = (int)MathF.Ceiling(rate / e.Frequency);
			e.Samples = PlotCycles * cycleLength;
			for (int s = 0; s < e.Samples; s++)
				PlotBuffer[s] = dsp.Next(global, unit, rate);
		}
	}
}