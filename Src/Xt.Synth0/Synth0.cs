using System;
using System.Windows;
using System.Windows.Threading;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	class Synth0 : Window
	{
		[STAThread]
		static void Main()
		{
			var window = new Synth0();
			var app = new Application();
			app.DispatcherUnhandledException += OnDispatcherUnhandledException;
			app.Run(window);
		}

		static void OnError(string message)
		{
			var window = Application.Current.MainWindow;
			var showError = new Action(() => MessageBox.Show(window, message, "Error")); ;
			window.Dispatcher.BeginInvoke(showError);
		}

		static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			OnError(e.Exception.Message);
			e.Handled = true;
		}

		readonly SynthModel _model = new SynthModel();

		Synth0()
		{
			Title = nameof(Synth0);
			Content = SynthUI.Make(_model);
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
		}
	}
}