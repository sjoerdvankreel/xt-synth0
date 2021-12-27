using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class TrackUI
	{
		public static UIElement Make(AppModel model)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Add(SynthUI.Make(model));
			result.Add(SequencerUI.Make(model));
			return result;
		}
	}
}