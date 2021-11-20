using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class SliderUI
	{
		internal const int SliderSize = 128;

		static readonly string[] Notes = new[] {
			"C", "C#", "D", "D#", "E", "F",
			"F#", "G", "G#", "A", "A#", "B"
		};

		static string Format(ParamInfo<int> info, int value)
		=> info.Type switch
		{
			ParamType.Note => Notes[value],
			ParamType.Int => value.ToString(),
			ParamType.Float => FormatFloat(info, value),
			_ => throw new ArgumentException()
		};

		static string FormatFloat(ParamInfo<int> info, int value)
		{
			double min = info.Min;
			double max = info.Max;
			return ((value - min) / (max - min)).ToString("P1").PadLeft(6, '0');
		}

		internal static void Add(Grid grid, Param<int> param, int row)
		{
			grid.Children.Add(MakeSlider(param, row));
			grid.Children.Add(MakeLabel(param, row));
			grid.Children.Add(UI.MakeLabel(param.Info.Name, row, 0));
			grid.Children.Add(UI.MakeLabel(Format(param.Info, param.Info.Min), row, 2));
			grid.Children.Add(UI.MakeLabel(Format(param.Info, param.Info.Max), row, 4));
		}

		static UIElement MakeSlider(Param<int> param, int row)
		{
			var result = UI.MakeElement<Slider>(row, 3);
			result.Width = SliderSize;
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.MouseRightButtonUp += OnSetExactValue;
			result.ToolTip = "Right-click to set exact value.";
			result.VerticalAlignment = VerticalAlignment.Center;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			return result;
		}

		static void OnSetExactValue(object sender, MouseButtonEventArgs e)
		{
			
		}

		static UIElement MakeLabel(Param<int> param, int row)
		{
			var result = UI.MakeElement<Label>(row, 1);
			var binding = Bind.To(param, v => $"({Format(param.Info, v)})");
			result.SetBinding(ContentControl.ContentProperty, binding);
			result.VerticalContentAlignment = VerticalAlignment.Top;
			return result;
		}
	}
}