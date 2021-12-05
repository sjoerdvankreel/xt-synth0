using System;
using System.Windows;
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

		static void Run()
		{
			var app = new Application();
			var window = new MainWindow(Model);
			app.Startup += OnAppStartup;
			app.DispatcherUnhandledException += OnDispatcherUnhandledException;
			app.Run(window);
		}

		static void OnDispatcherUnhandledException(
			object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			OnError(e.Exception);
			e.Handled = true;
		}

		static void OnAppStartup(object sender, StartupEventArgs e)
		{
			var window = (MainWindow)Application.Current.MainWindow;
			_engine = SetupEngine(window);
			LoadSettings(_engine).CopyTo(Model.Settings);
			MenuUI.ShowSettings += OnShowSettings;
		}

		static void OnShowSettings(object sender, EventArgs e)
		{
			SettingsUI.Show(Model.Settings);
			IO.SaveSettings(Model.Settings);
		}

		static AudioEngine SetupEngine(Window mainWindow)
		{
			var helper = new WindowInteropHelper(mainWindow);
			var result = AudioEngine.Create(helper.Handle);
			AudioModel.AddAsioDevices(result.AsioDevices);
			AudioModel.AddWasapiDevices(result.WasapiDevices);
			return result;
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

		static void OnError(Exception error)
		{
			IO.LogError(StartTime, error);
			var window = Application.Current.MainWindow;
			var showError = new Action(() => MessageBox.Show(window, error.Message,
				"Error", MessageBoxButton.OK, MessageBoxImage.Error));
			window.Dispatcher.BeginInvoke(showError);
		}
	}
}