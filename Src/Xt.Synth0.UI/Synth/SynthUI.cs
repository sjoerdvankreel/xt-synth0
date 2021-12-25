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
			result.Children.Add(MakeRight(model));
			return result;
		}

		static void AddDocked(
			Panel panel, UIElement element, Dock dock)
		{
			panel.Children.Add(element);
			element.SetValue(DockPanel.DockProperty, dock);
		}

		static UIElement MakeRight(AppModel model)
		{
			var result = new DockPanel();
			AddDocked(result, ControlUI.Make(model.Audio), Dock.Bottom);
			AddDocked(result, PatternUI.Make(model), Dock.Bottom);
			return result;
		}

		static UIElement MakeLeft(AppModel model)
		{
			var result = new DockPanel();
			foreach (var unit in model.Synth.Units)
				AddDocked(result, GroupUI.Make(model, unit), Dock.Top);
			AddDocked(result, GroupUI.Make(model, model.Synth.Amp), Dock.Top);
			AddDocked(result, GroupUI.Make(model, model.Synth.Global), Dock.Top);
			AddDocked(result, PlotUI.Make(model), Dock.Top);
			AddDocked(result, GroupUI.Make(model, model.Synth.Track), Dock.Top);
			return result;
		}
	}
}