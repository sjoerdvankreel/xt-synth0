using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

		readonly SynthModel _synth = new();
		readonly AudioModel _audio = new();
		readonly SettingsModel _settings = new();

		internal MainWindow()
		{
			AudioModel.AddAsioDevice("id1", "AsiName1");
			AudioModel.AddAsioDevice("id2", "AsiName2");
			AudioModel.AddWasapiDevice("id1", "WasName1");
			AudioModel.AddWasapiDevice("id2", "WasName2");
			MenuUI.New += (s, e) => New();
			MenuUI.Open += (s, e) => Load();
			MenuUI.Save += (s, e) => Save();
			MenuUI.SaveAs += (s, e) => SaveAs();
			MenuUI.Settings += (s, e) => Settings();
			ControlUI.Stop += (s, e) => _audio.IsRunning = false;
			ControlUI.Start += (s, e) => _audio.IsRunning = true;
			BindTitle();
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
			_synth.ParamChanged += (s, e) => IsDirty = true;
			BindCommand(ApplicationCommands.New, (s, e) => New());
			BindCommand(ApplicationCommands.Open, (s, e) => Load());
			BindCommand(ApplicationCommands.Save, (s, e) => Save());
			BindCommand(ApplicationCommands.SaveAs, (s, e) => SaveAs());
			Loaded += OnLoaded;
		}

		void OnLoaded(object sender, RoutedEventArgs _)
		{
			try
			{
				IO.LoadSetting(_settings);
			}
			catch (Exception e)
			{
				Synth0.OnError(e);
			}
			Loaded -= OnLoaded;
		}

		void Settings()
		{
			SettingsUI.Show(_settings);
			IO.SaveSettings(_settings);
		}

		internal void SaveAs()
		=> Save(LoadSaveUI.Save());
		internal void Save()
		=> Save(Path ?? LoadSaveUI.Save());

		void Save(string path)
		{
			if (path == null) return;
			IO.SaveFile(_synth, path);
			Path = path;
			IsDirty = false;
		}

		internal void Load()
		{
			if (!SaveUnsavedChanges()) return;
			var path = LoadSaveUI.Load();
			if (path == null) return;
			IO.LoadFile(path, _synth);
			Path = path;
			IsDirty = false;
		}

		internal void New()
		{
			if (!SaveUnsavedChanges()) return;
			new SynthModel().CopyTo(_synth);
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
			var control = ControlUI.Make(_audio);
			control.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(control);
			var synth = SynthUI.Make(_synth, _settings, _audio);
			synth.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(synth);
			result.SetValue(TextBlock.FontFamilyProperty, Utility.FontFamily);
			result.Resources = Utility.GetThemeResources(_settings.Theme);

			_settings.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == nameof(SettingsModel.Theme))
					result.Resources = Utility.GetThemeResources(_settings.Theme);
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