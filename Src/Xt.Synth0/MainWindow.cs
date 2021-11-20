using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xt.Synth0.Model;
using Xt.Synth0.UI;

namespace Xt.Synth0
{
	class MainWindow : Window
	{
		static readonly DependencyPropertyKey ModelPropertyKey
		= DependencyProperty.RegisterReadOnly("Model", typeof(SynthModel),
			typeof(MainWindow), new PropertyMetadata(new SynthModel()));
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

		public SynthModel Model
		=> (SynthModel)GetValue(ModelPropertyKey.DependencyProperty);

		internal MainWindow()
		{
			BindTitle();
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SizeToContent = SizeToContent.WidthAndHeight;
			Model.ParamChanged += (s, e) => IsDirty = true;
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
			IO.Save(Model, path);
			Path = path;
			IsDirty = false;
		}

		internal void Load()
		{
			var path = LoadSaveUI.Load();
			if (path == null) return;
			IO.Load(path, Model);
			Path = path;
			IsDirty = false;
		}

		UIElement MakeContent()
		{
			var result = new DockPanel();
			var menu = MenuUI.Make();
			menu.SetValue(DockPanel.DockProperty, Dock.Top);
			result.Children.Add(menu);
			var synth = SynthUI.Make(Model);
			synth.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(synth);
			return result;
		}

		void BindTitle()
		{
			var path = Bind.To(this, nameof(Path));
			var dirty = Bind.To(this, nameof(IsDirty));
			SetBinding(TitleProperty, Bind.Of(FormatTitle, path, dirty));
		}

		void BindCommand(RoutedUICommand command, EventHandler handler)
		{
			if (command.InputGestures.Count > 0)
				InputBindings.Add(new InputBinding(command, command.InputGestures[0]));
			CommandBindings.Add(new CommandBinding(command, (s, e) => handler(this, EventArgs.Empty)));
		}
	}
}