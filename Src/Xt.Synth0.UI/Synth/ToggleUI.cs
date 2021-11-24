using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class ToggleUI
	{
		internal static void Add(
			Grid grid, Param param, GridSettings settings)
		{
			grid.Children.Add(UI.MakeLabel(param.Info.Name, settings));
			grid.Children.Add(MakeCheckbox(param, new(settings.Row, settings.Col + 2)));
		}

		static UIElement MakeCheckbox(
			Param param, GridSettings settings)
		{
			var result = UI.MakeElement<CheckBox>(settings);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.VerticalAlignment = VerticalAlignment.Center;
			return result;
		}
	}
}