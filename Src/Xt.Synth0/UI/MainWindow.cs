using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	class MainWindow : Window
	{ 
		static readonly DependencyProperty PathProperty
		= DependencyProperty.Register("Path", typeof(string), typeof(MainWindow));
		static readonly DependencyProperty IsDirtyProperty
		= DependencyProperty.Register("IsDirty", typeof(bool), typeof(MainWindow));

		static string FormatTitle(object[] args)
		{
			var result = args[0] == null ? nameof(Synth0)
				: $"{nameof(Synth0)} ({args[0]})";
			return (bool)args[1] ? $"{result} *" : result;
		}

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

		readonly SynthModel _model = new();

		internal MainWindow()
		{
			BindTitle();
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
			_model.ParamChanged += (s, e) => IsDirty = true;
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
			IO.Save(_model, path);
			Path = path;
			IsDirty = false;
		}

		internal void Load()
		{
			if (!SaveUnsavedChanges()) return;
			var path = LoadSaveUI.Load();
			if (path == null) return;
			IO.Load(path, _model);
			Path = path;
			IsDirty = false;
		}

		internal void New()
		{
			if (!SaveUnsavedChanges()) return;
			new SynthModel().CopyTo(_model);
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
			var control = ControlUI.Make();
			control.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(control);
			var synth = SynthUI.Make(_model);
			synth.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(synth);
			result.SetValue(TextBlock.FontFamilyProperty, new FontFamily("Consolas"));


			result.Resources = ConvertTheme(ThemeType.None);

			return result;
		}

		internal ResourceDictionary ConvertTheme(ThemeType value)
		{
			ResourceDictionary result = new();
			result.Source = new Uri(ResourceUri(value));
			return result;
		}

		string ResourceUri(ThemeType type) => type switch
		{
			ThemeType.Red => "pack://application:,,,/Xt.Synth0.UI;component/Themes/Red.xaml",
			ThemeType.Blue => "pack://application:,,,/Xt.Synth0.UI;component/Themes/Blue.xaml",
			ThemeType.Green => "pack://application:,,,/Xt.Synth0.UI;component/Themes/Green.xaml",
			ThemeType.None => "pack://application:,,,/Xt.Synth0.UI;component/Themes/Generic.xaml",
			_ => throw new InvalidOperationException()
		};

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
			var path = Bind(this, nameof(Path));
			var dirty = Bind(this, nameof(IsDirty));
			SetBinding(TitleProperty, Bind(FormatTitle, path, dirty));
		}

		void BindCommand(RoutedUICommand command, EventHandler handler)
		{
			if (command.InputGestures.Count > 0)
				InputBindings.Add(new InputBinding(command, command.InputGestures[0]));
			CommandBindings.Add(new CommandBinding(command, (s, e) => handler(this, EventArgs.Empty)));
		}

		Binding Bind(object source, string path)
		{
			var result = new Binding(path);
			result.Source = source;
			return result;
		}

		MultiBinding Bind(
			Func<object[], string> format, params Binding[] bindings)
		{
			var result = new MultiBinding();
			foreach (var binding in bindings)
				result.Bindings.Add(binding);
			result.Converter = new MultiFormatter(format);
			return result;
		}
	}
}