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

		static void Load(MainWindow window)
		{
			if (!SaveUnsavedChanges(window)) return;
			var path = LoadSaveUI.Load();
			if (path == null) return;
			IO.LoadFile(path, Model.Synth);
			window.SetClean(path);
		}

		static void New(MainWindow window)
		{
			if (!SaveUnsavedChanges(window)) return;
			new SynthModel().CopyTo(Model.Synth);
			window.SetClean(null);
		}

		static void Save(MainWindow window, string path)
		{
			if (path == null) return;
			IO.SaveFile(Model.Synth, path);
			window.SetClean(path);
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

		static AudioEngine SetupEngine(Window mainWindow)
		{
			var helper = new WindowInteropHelper(mainWindow);
			var logger = (string msg) => IO.LogError(StartTime, msg, null);
			Action<Action> dispatch = a => Application.Current?.Dispatcher.BeginInvoke(a);
			var result = AudioEngine.Create(Model, helper.Handle, logger, dispatch);
			AudioModel.AddAsioDevices(result.AsioDevices);
			AudioModel.AddWasapiDevices(result.WasapiDevices);
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
			ControlUI.Stop += (s, e) => _engine.Stop();
			ControlUI.Start += (s, e) => _engine.Start(Model.Settings);
			Action showPanel = () => _engine.ShowASIOControlPanel(Model.Settings.AsioDeviceId);
			SettingsUI.ShowASIOControlPanel += (s, e) => showPanel();
		}
	}
}