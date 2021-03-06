using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class MenuUI
	{
		public static event EventHandler New;
		public static event EventHandler Open;
		public static event EventHandler Save;
		public static event EventHandler SaveAs;
		public static event EventHandler ShowSettings;
		public static event EventHandler<OpenRecentEventArgs> OpenRecent;

		public static UIElement Make(AppModel app)
		{
			var result = new Menu();
			result.Items.Add(MakeFile(app));
			return result;
		}

		static MenuItem MakeItem(string header)
		{
			var result = new MenuItem();
			result.Header = header;
			return result;
		}

		static MenuItem MakeItem(string header, Action execute)
		{
			var result = new MenuItem();
			result.Header = header;
			result.Click += (s, e) => execute();
			return result;
		}

		static MenuItem MakeItem(
			ICommand command, string header, Action execute)
		{
			var result = MakeItem(header);
			result.Command = command;
			var binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += (s, e) => execute();
			result.CommandBindings.Add(binding);
			return result;
		}

		static MenuItem MakeItem(StreamModel stream, 
			string header, Action execute)
		{
			var result = MakeItem(header, execute);
			var binding = Bind.To(stream, nameof(stream.IsStopped));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static MenuItem MakeItem(ICommand command,
			StreamModel stream, string header, Action execute)
		{
			var result = MakeItem(command, header, execute);
			var binding = Bind.To(stream, nameof(stream.IsStopped));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static MenuItem MakeRecent(AppModel app)
		{
			var result = MakeItem("Recent Files");
			result.Click += OnRecentFileClick;
			result.ItemsSource = app.Settings.RecentFiles; 
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsStopped));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			binding = Bind.Show(app.Settings.RecentFiles, nameof(ICollection.Count), 1);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static void OnRecentFileClick(object sender, RoutedEventArgs e)
		{
			var source = e.OriginalSource as MenuItem;
			if (source == null || source == sender) return;
			OpenRecent?.Invoke(null, new OpenRecentEventArgs(source.Header.ToString()));
		}

		static UIElement MakeFile(AppModel app)
		{
			var stream = app.Stream;
			var result = MakeItem("_File");
			var doNew = () => New(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.New, stream, "_New", doNew));
			var doOpen = () => Open(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.Open, stream, "_Open", doOpen));
			result.Items.Add(new Separator());
			var doSave = () => Save(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.Save, "_Save", doSave));
			var doSaveAs = () => SaveAs(null, EventArgs.Empty);
			result.Items.Add(MakeItem(ApplicationCommands.SaveAs, "Save _As", doSaveAs));
			result.Items.Add(new Separator());
			var doShowSettings = () => ShowSettings(null, EventArgs.Empty);
			result.Items.Add(MakeItem(stream, "Settings", doShowSettings));
			result.Items.Add(MakeRecent(app));
			result.Items.Add(new Separator());
			var doExit = () => Application.Current.MainWindow.Close();
			result.Items.Add(MakeItem("E_xit", doExit));
			return result;
		}
	}
}