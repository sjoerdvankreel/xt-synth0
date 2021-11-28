using System;
using System.Windows;
using System.Windows.Threading;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	static class Synth0
	{
		internal static DateTime StartTime { get; private set; }

		[STAThread]
		static void Main()
		{
			StartTime = DateTime.Now;
			var app = new Application();
			var resources = new ResourceDictionary();
			resources.Source = new Uri("pack://application:,,,/Xt.Synth0.UI;component/Themes/Blue.xaml");
			app.Resources = resources;
			var window = new MainWindow();
			MenuUI.New = window.New;
			MenuUI.Load = window.Load;
			MenuUI.Save = window.Save;
			MenuUI.SaveAs = window.SaveAs;
			app.DispatcherUnhandledException += OnDispatcherUnhandledException;
			app.Run(window);
		}

		static void OnDispatcherUnhandledException(object sender,
			DispatcherUnhandledExceptionEventArgs e)
		{
			OnError(e.Exception.Message);
			e.Handled = true;
		}

		static void OnError(string message)
		{
			IO.LogError(message);
			var window = Application.Current.MainWindow;
			var showError = new Action(() => MessageBox.Show(window, message,
				"Error", MessageBoxButton.OK, MessageBoxImage.Error));
			window.Dispatcher.BeginInvoke(showError);
		}
	}
}