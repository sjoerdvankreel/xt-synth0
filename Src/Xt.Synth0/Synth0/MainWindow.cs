using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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

		AppModel Model { get; }

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

		internal void SetClean(string path)
		=> (IsDirty, Path) = (false, path);

		internal MainWindow(AppModel model)
		{
			Model = model;
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SetBinding(TitleProperty, BindTitle());
			SizeToContent = SizeToContent.WidthAndHeight;
			Model.Synth.ParamChanged += (s, e) => IsDirty = true;
		}

		BindingBase BindTitle()
		{
			var pathBinding = Bind.To(this, nameof(Path));
			var dirtyBinding = Bind.To(this, nameof(IsDirty));
			return Bind.To(pathBinding, dirtyBinding, new TitleFormatter());
		}

		UIElement MakeContent()
		{
			var result = new DockPanel();
			var menu = MenuUI.Make(Model.Audio);
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
			Model.Settings.ThemeChanged += (s, e) => result.Resources 
				= Utility.GetThemeResources(Model.Settings.Theme);
			return result;
		}
	}
}