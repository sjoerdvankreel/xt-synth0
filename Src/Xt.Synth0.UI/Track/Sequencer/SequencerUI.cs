using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class SequencerUI
	{
		internal static UIElement Make(AppModel model)
		{
			var result = Create.Grid(3, 1);
			var edit = model.Track.Sequencer.Edit;
			result.Add(PatternUI.Make(model), new Cell(0, 0));
			result.Add(EditUI.Make(model, edit), new(1, 0));
			result.Add(MonitorUI.Make(model.Stream), new Cell(1, 0));
			result.Add(ControlUI.Make(model.Stream), new Cell(2, 0));
			return result;
		}
	}
}