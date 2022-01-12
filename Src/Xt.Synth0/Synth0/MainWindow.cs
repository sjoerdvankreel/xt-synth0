using System;
using System.Windows;
using System.Windows.Automation.Peers;
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
		readonly NoAutomationPeer _automationPeer;

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
		protected override AutomationPeer OnCreateAutomationPeer()
		=> _automationPeer;

		internal MainWindow(AppModel model)
		{
			Model = model;
			Content = MakeContent();
			ResizeMode = ResizeMode.NoResize;
			SetBinding(TitleProperty, BindTitle());
			SizeToContent = SizeToContent.WidthAndHeight;
			_automationPeer = new NoAutomationPeer(this);
			Model.Track.ParamChanged += OnTrackParamChanged;
		}

		void OnTrackParamChanged(object sender, EventArgs e)
		{
			if (!Model.Stream.IsRunning)
				IsDirty = true;
		}

		BindingBase BindTitle()
		{
			var path = Bind.To(this, nameof(Path));
			var dirty = Bind.To(this, nameof(IsDirty));
			return Bind.To(new TitleFormatter(), path, dirty);
		}

		UIElement MakeContent()
		{
			var result = Create.Themed<DockPanel>(Model.Settings, ThemeGroup.Settings);
			var menu = result.Add(MenuUI.Make(Model));
			menu.SetValue(DockPanel.DockProperty, Dock.Top);
			var track = result.Add(TrackUI.Make(Model));
			track.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.SetValue(TextBlock.FontFamilyProperty, Utility.FontFamily);
			return result;
		}
	}
}