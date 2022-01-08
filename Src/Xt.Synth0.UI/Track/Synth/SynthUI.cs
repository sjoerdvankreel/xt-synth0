using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class SynthUI
	{
		internal static UIElement Make(AppModel model)
		{
			var result = new DockPanel();
			foreach (var unit in model.Track.Synth.Units)
				result.Add(GroupUI.Make(model, unit), Dock.Top);
			result.Add(GroupUI.Make(model, model.Track.Synth.Global), Dock.Top);
			result.Add(PlotUI.Make(model), Dock.Top);
			return result;
		}
	}
}