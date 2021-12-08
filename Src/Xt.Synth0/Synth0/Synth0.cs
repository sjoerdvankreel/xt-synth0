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
		static AudioEngine _engine;
		static readonly AppModel Model = new AppModel();
		static readonly DateTime StartTime = DateTime.Now;

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

		static void CopyToUIThread(SynthModel model)
		{
			if (Model.Audio.IsRunning)
				model.CopyTo(Model.Synth);
			ModelPool.Return(model);
		}

		static void Run()
		{
			var app = new Application();
			app.Startup += OnAppStartup;
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
			new SynthModel().CopyTo(Model.Synth);
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
			MenuUI.New += (s, e) => New(window);
			MenuUI.Open += (s, e) => Load(window);
			MenuUI.Save += (s, e) => Save(window);
			MenuUI.SaveAs += (s, e) => SaveAs(window);
			MenuUI.ShowSettings += (s, e) => ShowSettings();
			MenuUI.OpenRecent += (s, e) => LoadRecent(window, e.Path);
			ControlUI.Stop += (s, e) => _engine.Stop();
			ControlUI.Start += (s, e) => _engine.Start();
			Action showPanel = () => _engine.ShowASIOControlPanel(Model.Settings.AsioDeviceId);
			SettingsUI.ShowASIOControlPanel += (s, e) => showPanel();
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
	}
}