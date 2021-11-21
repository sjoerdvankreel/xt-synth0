using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(SynthModel model)
		{
			var result = UI.MakeGrid(4, 3);
			int halfLength = PatternModel.Length / 2;
			result.Margin = new Thickness(UI.Margin);
			result.Children.Add(GroupUI.Make(model.Unit1, nameof(model.Unit1), 0, 0));
			result.Children.Add(GroupUI.Make(model.Unit2, nameof(model.Unit2), 1, 0));
			result.Children.Add(GroupUI.Make(model.Unit3, nameof(model.Unit3), 2, 0));
			result.Children.Add(GroupUI.Make(model.Global, nameof(model.Global), 3, 0));
			result.Children.Add(PatternUI.Make(model.Pattern, "Pattern1", 0, halfLength, 0, 1, 4));
			result.Children.Add(PatternUI.Make(model.Pattern, "Pattern2", halfLength, halfLength, 0, 2, 4));
			return result;
		}
	}
}