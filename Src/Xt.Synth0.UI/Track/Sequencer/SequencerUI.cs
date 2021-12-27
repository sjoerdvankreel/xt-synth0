using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class SequencerUI
	{
		internal static UIElement Make(AppModel model)
		{
			var result = new DockPanel();
			var edit = model.Track.Sequencer.Edit;
			result.Add(GroupUI.MakeStatic(model, edit), Dock.Bottom);
			result.Add(PatternUI.Make(model), Dock.Bottom);
			return result;
		}
	}
}