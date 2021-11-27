using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class ParamUI
	{
		internal static void Add(
			Grid grid, Param param, Cell cell)
		{
			grid.Children.Add(MakeName(param, cell.Right(1)));
			grid.Children.Add(MakeValue(param, cell.Right(2)));
			if (!param.Info.IsToggle)
				grid.Children.Add(MakeKnob(param, cell));
			else
				grid.Children.Add(MakeToggle(param, cell));
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
			var binding = UI.Format(param);
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeToggle(Param param, Cell cell)
		{
			var result = UI.MakeElement<Toggle>(cell);
			result.ToolTip = param.Info.Detail;
			result.SetBinding(ToggleButton.IsCheckedProperty, UI.Bind(param));
			return result;
		}

		static UIElement MakeKnob(Param param, Cell cell)
		{
			var result = UI.MakeElement<Knob>(cell);
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.MouseRightButtonUp += (s, e) => EditUI.Show(param);
			result.SetBinding(RangeBase.ValueProperty, UI.Bind(param));
			result.ToolTip = param.Info.Detail + " (right-click to set exact value)";
			return result;
		}
	}
}