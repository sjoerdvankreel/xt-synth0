using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	class Synth0 : Window
	{
		public static object GetPath(DependencyObject obj)
		=> obj.GetValue(PathProperty);
		public static void SetPath(DependencyObject obj, object value)
		=> obj.SetValue(PathProperty, value);
		static readonly DependencyProperty PathProperty
		= DependencyProperty.Register("Path", typeof(string), typeof(Synth0));

		static string FormatTitle(string path)
		=> path == null ? nameof(Synth0) : $"{nameof(Synth0)} ({path})";

		[STAThread]
		static void Main()
		{
			var startTime = DateTime.Now;
			var window = new Synth0(startTime);
			var app = new Application();
			MenuUI.Load += window.OnLoad;
			MenuUI.Save += window.OnSave;
			MenuUI.SaveAs += window.OnSaveAs;
			app.DispatcherUnhandledException += window.OnDispatcherUnhandledException;
			app.Run(window);
		}

		public string Path
		{
			get => (string)GetValue(PathProperty);
			set => SetValue(PathProperty, value);
		}

		readonly DateTime _startTime;
		readonly SynthModel _model = new SynthModel();

		Synth0(DateTime startTime)
		{
			_startTime = startTime;
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
			BindCommand(ApplicationCommands.Open, OnLoad);
			BindCommand(ApplicationCommands.Save, OnSave);
			BindCommand(ApplicationCommands.SaveAs, OnSaveAs);
			SetBinding(TitleProperty, Bind.To<string>(this, nameof(Path), FormatTitle));
		}

		void OnSaveAs(object sender, EventArgs e)
		=> Save(LoadSaveUI.Save());

		void OnSave(object sender, EventArgs e)
		=> Save(Path ?? LoadSaveUI.Save());

		UIElement MakeContent()
		{
			var result = new DockPanel();
			var menu = MenuUI.Make();
			menu.SetValue(DockPanel.DockProperty, Dock.Top);
			result.Children.Add(menu);
			var synth = SynthUI.Make(_model);
			synth.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(synth);
			return result;
		}

		void Save(string path)
		{
			if (path == null) return;
			IO.Save(_model, path);
			Path = path;
		}

		void OnLoad(object sender, EventArgs e)
		{
			var path = LoadSaveUI.Load();
			if (path == null) return;
			IO.Load(path, _model);
			Path = path;
		}

		void OnError(string message)
		{
			IO.LogError(_startTime, message);
			var window = Application.Current.MainWindow;
			var showError = new Action(() => MessageBox.Show(window, message, "Error"));
			window.Dispatcher.BeginInvoke(showError);
		}

		void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			OnError(e.Exception.Message);
			e.Handled = true;
		}

		void BindCommand(RoutedUICommand command, EventHandler handler)
		{
			if (command.InputGestures.Count > 0)
				InputBindings.Add(new InputBinding(command, command.InputGestures[0]));
			CommandBindings.Add(new CommandBinding(command, (s, e) => handler(this, EventArgs.Empty)));
		}
	}
}