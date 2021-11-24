using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(SynthModel model)
		{
			var result = UI.MakeGrid(4, 3);
			result.Margin = new(UI.Margin);
			int rows = PatternModel.Length / 2;
			result.Children.Add(GroupUI.Make(model.Unit1, "Unit1", new(0, 0)));
			result.Children.Add(GroupUI.Make(model.Unit2, "Unit2", new(1, 0)));
			result.Children.Add(GroupUI.Make(model.Unit3, "Unit3", new(2, 0)));
			result.Children.Add(GroupUI.Make(model.Global, "Global", new(3, 0)));
			result.Children.Add(PatternUI.Make(model.Pattern, "Pattern1", 0, rows, new(0, 1, 4)));
			result.Children.Add(PatternUI.Make(model.Pattern, "Pattern2", rows, rows, new(0, 2, 4)));
			return result;
		}
	}
}