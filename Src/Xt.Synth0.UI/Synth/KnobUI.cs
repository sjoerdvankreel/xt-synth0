using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class KnobUI
	{
		internal const int KnobSize = 16;

		static UIElement MakeLabel(Param param, GridSettings settings)
		{
			var result = UI.MakeElement<Label>(settings);
			var binding = Bind.To(param, v => $"{param.Info.Format(v)}");
			result.SetBinding(ContentControl.ContentProperty, binding);
			result.VerticalContentAlignment = VerticalAlignment.Top;
			return result;
		}

		static UIElement MakeKnob(Param param, GridSettings settings)
		{
			var result = UI.MakeElement<Knob>(settings);
			result.Width = KnobSize;
			result.Height = KnobSize;
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.RotaryFill = Brushes.Gray;
			result.MarkerFill = Brushes.Black;
			result.RotaryStroke = Brushes.Black;
			result.MarkerStroke = Brushes.Black;
			result.MarkerSize = KnobSize / 3.0;
			result.VerticalAlignment = VerticalAlignment.Center;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(FrameworkElement.ToolTipProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => EditUI.Show(param);
			return result;
		}

		internal static void Add(Grid grid, Param param, GridSettings settings)
		{
			grid.Children.Add(UI.MakeLabel(param.Info.Name, settings));
			grid.Children.Add(MakeKnob(param, new(settings.Row, settings.Col + 2)));
			grid.Children.Add(MakeLabel(param, new(settings.Row, settings.Col + 1)));
		}
	}
}