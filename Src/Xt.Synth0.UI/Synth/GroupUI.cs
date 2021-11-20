using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class GroupUI
	{
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

		internal static UIElement Make(IGroupModel model, string name, 
			int row, int column, int rowSpan = 1, int columnSpan = 1)
		{
			var result = UI.MakeElement<GroupBox>(row, column, rowSpan, columnSpan);
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}
	}
}