using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class ToggleUI
	{
		internal static void Add(Grid grid, Param<bool> param, int row)
		{
			grid.Children.Add(MakeCheckbox(param, row));
			grid.Children.Add(UI.MakeLabel(param.Info.Name, row, 0));
		}

		static UIElement MakeCheckbox(Param<bool> param, int row)
		{
			var result = UI.MakeElement<CheckBox>(row, 2);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.VerticalAlignment = VerticalAlignment.Center;
			return result;
		}
	}
}