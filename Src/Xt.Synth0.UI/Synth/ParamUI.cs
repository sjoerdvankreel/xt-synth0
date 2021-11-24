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
			var binding = Bind.To(param, v => $"{param.Info.Format(v)}");
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeCheckbox(Param param, Cell cell)
		{
			var result = UI.MakeElement<CheckBox>(cell);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			return result;
		}

		static UIElement MakeKnob(Param param, Cell cell)
		{
			var result = UI.MakeElement<Knob>(cell);
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(FrameworkElement.ToolTipProperty, Bind.To(param));
			return result;
		}
	}
}