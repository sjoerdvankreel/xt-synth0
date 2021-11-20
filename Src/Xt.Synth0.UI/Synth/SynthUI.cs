using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(SynthModel model)
		{
			var result = UI.MakeGrid(2, 3);
			result.Margin = new Thickness(UI.Margin);
			result.Children.Add(GroupUI.Make(model.Unit1, nameof(model.Unit1), 0, 0));
			result.Children.Add(GroupUI.Make(model.Unit2, nameof(model.Unit2), 0, 1));
			result.Children.Add(GroupUI.Make(model.Unit3, nameof(model.Unit3), 0, 2));
			result.Children.Add(GroupUI.Make(model.Main, nameof(model.Main), 1, 0));
			return result;
		}
	}
}