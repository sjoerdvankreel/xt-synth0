using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class ToggleUI
	{
		internal static void Add(Grid grid, Param param, int row, int column)
		{
			grid.Children.Add(MakeCheckbox(param, row, column + 2));
			grid.Children.Add(UI.MakeLabel(param.Info.Name, row, column));
		}

		static UIElement MakeCheckbox(Param param, int row, int column)
		{
			var result = UI.MakeElement<CheckBox>(row, column);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.VerticalAlignment = VerticalAlignment.Center;
			return result;
		}
	}
}