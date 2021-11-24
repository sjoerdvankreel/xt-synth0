using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class ParamUI
	{
		internal const int KnobSize = 16;

		internal static void Add(
			Grid grid, Param param, Cell cell)
		{
			var editor = param.Info.Type != ParamType.Toggle
				? MakeKnob(param, cell)
				: MakeCheckbox(param, cell);
			editor.MouseRightButtonUp += (s, e) => EditUI.Show(param);
			grid.Children.Add(editor);
			grid.Children.Add(MakeLabel(param, cell.Right(2)));
			grid.Children.Add(UI.MakeLabel(param.Info.Name, cell.Right(1)));
		}

		static UIElement MakeLabel(Param param, Cell cell)
		{
			var result = UI.MakeElement<Label>(cell);
			result.VerticalContentAlignment = VerticalAlignment.Top;
			var binding = Bind.To(param, v => $"{param.Info.Format(v)}");
			result.SetBinding(ContentControl.ContentProperty, binding);
			result.HorizontalContentAlignment = HorizontalAlignment.Right;
			return result;
		}

		static UIElement MakeCheckbox(Param param, Cell cell)
		{
			var result = UI.MakeElement<CheckBox>(cell);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.VerticalAlignment = VerticalAlignment.Center;
			return result;
		}

		static UIElement MakeKnob(Param param, Cell cell)
		{
			var result = UI.MakeElement<Knob>(cell);
			result.Width = KnobSize;
			result.Height = KnobSize;
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Center;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(FrameworkElement.ToolTipProperty, Bind.To(param));
			return result;
		}
	}
}