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
			var window = new MainWindow();
			MenuUI.Load += (s, e) => window.Load();
			MenuUI.Save += (s, e) => window.Save();
			MenuUI.SaveAs += (s, e) => window.SaveAs();
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