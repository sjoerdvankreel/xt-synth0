using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class GroupUI
	{
		internal static UIElement Make(IGroupModel model, string name, int row, int column)
		{
			var result = UI.MakeElement<GroupBox>(row, column);
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}

		static UIElement MakeContent(IGroupModel model)
		{
			var ints = model.IntParams();
			var bools = model.BoolParams();
			var result = UI.MakeGrid(ints.Length + bools.Length, 5);
			for (int r = 0; r < bools.Length; r++)
				ToggleUI.Add(result, bools[r], r);
			for (int r = 0; r < ints.Length; r++)
				SliderUI.Add(result, ints[r], bools.Length + r);
			return result;
		}
	}
}