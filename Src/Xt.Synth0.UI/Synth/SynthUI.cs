using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(SynthModel model)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(MakeLeft(model));
			result.Children.Add(MakeRight(model));
			return result;
		}

		static UIElement MakeLeft(SynthModel model)
		{
			var result = new StackPanel();
			result.Children.Add(GroupUI.Make(model.Unit1, nameof(model.Unit1)));
			result.Children.Add(GroupUI.Make(model.Unit2, nameof(model.Unit2)));
			result.Children.Add(GroupUI.Make(model.Unit3, nameof(model.Unit3)));
			result.Children.Add(GroupUI.Make(model.Editor, nameof(model.Editor)));
			return result;
		}

		static UIElement MakeRight(SynthModel model)
		{
			var result = new DockPanel();
			var pattern = PatternUI.Make(model.Pattern, model.Editor, nameof(model.Pattern));
			pattern.SetValue(DockPanel.DockProperty, Dock.Top);
			result.Children.Add(pattern);
			var global = GroupUI.Make(model.Global, nameof(model.Global));
			global.SetValue(DockPanel.DockProperty, Dock.Bottom);
			result.Children.Add(global);
			return result;
		}
	}
}