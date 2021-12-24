using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(AppModel model)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(MakeLeft(model));
			result.Children.Add(PatternUI.Make(model));
			return result;
		}

		static void AddDocked(
			Panel panel, UIElement element)
		{
			panel.Children.Add(element);
			element.SetValue(DockPanel.DockProperty, Dock.Top);
		}

		static UIElement MakeLeft(AppModel model)
		{
			var result = new DockPanel();
			foreach (var unit in model.Synth.Units)
				AddDocked(result, GroupUI.Make(model, unit));
			AddDocked(result, GroupUI.Make(model, model.Synth.Amp));
			AddDocked(result, GroupUI.Make(model, model.Synth.Global));
			AddDocked(result, GroupUI.Make(model, model.Synth.Track));
			AddDocked(result, PlotUI.Make(model));
			return result;
		}
	}
}