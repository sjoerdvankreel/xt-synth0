using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	class MainWindow : Window
	{
		static readonly DependencyProperty PathProperty
		= DependencyProperty.Register(nameof(Path), typeof(string), typeof(MainWindow));
		static readonly DependencyProperty IsDirtyProperty
		= DependencyProperty.Register(nameof(IsDirty), typeof(bool), typeof(MainWindow));

		public string Path
		{
			get => (string)GetValue(PathProperty);
			set => SetValue(PathProperty, value);
		}

		public bool IsDirty
		{
			get => (bool)GetValue(IsDirtyProperty);
			set => SetValue(IsDirtyProperty, value);
		}

		AppModel Model { get; }

		internal MainWindow(AppModel model)
		{
			Model = model;
			MenuUI.New += (s, e) => New();
			MenuUI.Open += (s, e) => Load();
			MenuUI.Save += (s, e) => Save();
			MenuUI.SaveAs += (s, e) => SaveAs();
			ControlUI.Stop += (s, e) => Model.Audio.IsRunning = false;
			ControlUI.Start += (s, e) => Model.Audio.IsRunning = true;
			BindTitle();
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
			Model.Synth.ParamChanged += (s, e) => IsDirty = true;
			BindCommand(ApplicationCommands.New, (s, e) => New());
			BindCommand(ApplicationCommands.Open, (s, e) => Load());
			BindCommand(ApplicationCommands.Save, (s, e) => Save());
			BindCommand(ApplicationCommands.SaveAs, (s, e) => SaveAs());
		}

		internal void SaveAs()
		=> Save(LoadSaveUI.Save());
		internal void Save()
		=> Save(Path ?? LoadSaveUI.Save());

		void Save(string path)
		{
			if (path == null) return;
			IO.SaveFile(Model.Synth, path);
			Path = path;
			IsDirty = false;
		}

		internal void Load()
		{
			if (!SaveUnsavedChanges()) return;
			var path = LoadSaveUI.Load();
			if (path == null) return;
			IO.LoadFile(path, Model.Synth);
			Path = path;
			IsDirty = false;
		}

		internal void New()
		{
			if (!SaveUnsavedChanges()) return;
			new SynthModel().CopyTo(Model.Synth);
			Path = null;
			IsDirty = false;
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			e.Cancel = !SaveUnsavedChanges();
			base.OnClosing(e);
		}

		UIElement MakeContent()
		{
			var result = new DockPanel();
			var menu = MenuUI.Make();
			menu.SetValue(DockPanel.DockProperty, Dock.Top);
			result.Children.Add(menu);
			var control = ControlUI.Make(Model.Audio);
			control.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(control);
			var synth = SynthUI.Make(Model);
			synth.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(synth);
			result.SetValue(TextBlock.FontFamilyProperty, Utility.FontFamily);
			result.Resources = Utility.GetThemeResources(Model.Settings.Theme);

			Model.Settings.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(SettingsModel.Theme))
					result.Resources = Utility.GetThemeResources(Model.Settings.Theme);
			};
			return result;
		}

		bool SaveUnsavedChanges()
		{
			if (!IsDirty) return true;
			var result = MessageBox.Show(this, "Save changes?",
				"Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
			if (result == MessageBoxResult.Cancel) return false;
			if (result == MessageBoxResult.Yes) Save();
			return true;
		}

		void BindTitle()
		{
			var binding = Bind.To(this, nameof(Path), this, nameof(IsDirty), new TitleFormatter());
			SetBinding(TitleProperty, binding);
		}

		void BindCommand(RoutedUICommand command, EventHandler handler)
		{
			if (command.InputGestures.Count > 0)
				InputBindings.Add(new InputBinding(command, command.InputGestures[0]));
			CommandBindings.Add(new CommandBinding(command, (s, e) => handler(this, EventArgs.Empty)));
		}
	}
}