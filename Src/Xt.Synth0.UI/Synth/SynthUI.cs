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

		static void AddDocked(Panel panel, UIElement element)
		{
			panel.Children.Add(element);
			element.SetValue(DockPanel.DockProperty, Dock.Top);
		}

		static UIElement MakeLeft(SynthModel model)
		{
			var result = new DockPanel();
			AddDocked(result, GroupUI.Make(model.Unit1, nameof(model.Unit1)));
			AddDocked(result, GroupUI.Make(model.Unit2, nameof(model.Unit2)));
			AddDocked(result, GroupUI.Make(model.Unit3, nameof(model.Unit3)));
			AddDocked(result, GroupUI.Make(model.Editor, nameof(model.Editor)));
			return result;
		}

		static UIElement MakeRight(SynthModel model)
		{
			var result = new DockPanel();
			AddDocked(result, PatternUI.Make(model.Pattern, model.Editor, nameof(model.Pattern)));
			AddDocked(result, GroupUI.Make(model.Global, nameof(model.Global)));
			return result;
		}
	}
}