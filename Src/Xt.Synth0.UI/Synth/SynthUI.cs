using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(SynthModel model)
		{
			var result = UI.MakeGrid(4, 3);
			result.Children.Add(GroupUI.Make(model.Unit1, "Unit1", new(0, 0)));
			result.Children.Add(GroupUI.Make(model.Unit2, "Unit2", new(1, 0)));
			result.Children.Add(GroupUI.Make(model.Unit3, "Unit3", new(2, 0)));
			result.Children.Add(GroupUI.Make(model.Global, "Global", new(3, 0)));
			result.Children.Add(GroupUI.Make(model.Editor, "Editor", new(0, 1)));
			result.Children.Add(PatternUI.Make(model.Pattern, "Pattern", new(1, 1, 3)));
			return result;
		}
	}
}