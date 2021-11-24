using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class ParamUI
	{
		internal static void Add(
			Grid grid, Param param, Cell cell)
		{
			grid.Children.Add(MakeName(param, cell.Right(1)));
			grid.Children.Add(MakeValue(param, cell.Right(2)));
			if (param.Info.Type != ParamType.Toggle)
				grid.Children.Add(MakeKnob(param, cell));
			else
				grid.Children.Add(MakeCheckbox(param, cell));
		}

		static UIElement MakeName(Param param, Cell cell)
		{
			var result = UI.MakeElement<Label>(cell);
			result.Content = param.Info.Name;
			return result;
		}

		static UIElement MakeValue(Param param, Cell cell)
		{
			var result = UI.MakeElement<Label>(cell);
			var binding = Bind.To(param, v => $"{param.Info.Format(v)}");
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeCheckbox(Param param, Cell cell)
		{
			var result = UI.MakeElement<CheckBox>(cell);
			result.MouseRightButtonUp += (s, e) => EditUI.Show(param);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			return result;
		}

		static UIElement MakeKnob(Param param, Cell cell)
		{
			var result = UI.MakeElement<Knob>(cell);
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.MouseRightButtonUp += (s, e) => EditUI.Show(param);
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(FrameworkElement.ToolTipProperty, Bind.To(param));
			return result;
		}
	}
}