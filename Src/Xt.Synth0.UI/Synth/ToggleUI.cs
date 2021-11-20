using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class ToggleUI
	{
		static string Format(bool value) => value ? "On" : "Off";

		internal static void Add(Grid grid, Param<bool> param, int row)
		{
			grid.Children.Add(MakeCheckbox(param, row));
			grid.Children.Add(MakeLabel(param, row));
			grid.Children.Add(UI.MakeLabel(param.Info.Name, row, 0));
		}

		static UIElement MakeLabel(Param<bool> param, int row)
		{
			var result = UI.MakeElement<Label>(row, 1);
			var binding = Bind.To(param, v => $"({Format(v)})");
			result.SetBinding(ContentControl.ContentProperty, binding);
			result.VerticalContentAlignment = VerticalAlignment.Top;
			return result;
		}

		static UIElement MakeCheckbox(Param<bool> param, int row)
		{
			var result = UI.MakeElement<CheckBox>(row, 3);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.VerticalAlignment = VerticalAlignment.Center;
			return result;
		}
	}
}