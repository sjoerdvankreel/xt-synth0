﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternFxUI
	{
		internal static void Add(Grid grid, SynthModel synth, PatternFx fx,
			Param fxCount, int minFx, int row, int col, Action interpolate)
		{
			grid.Children.Add(MakeTarget(synth, fx.Target, fxCount, minFx, row, col));
			grid.Children.Add(MakeValue(fx.Value, fxCount, minFx, row, col + 1, interpolate));
		}

		static UIElement MakeValue(Param param, Param fxCount,
			int minFx, int row, int col, Action interpolate)
		{
			var result = MakeHex(param, fxCount, minFx, row, col);
			result.KeyDown += (s, e) => OnValueKeyDown(interpolate, e);
			result.ToolTip = string.Join("\n", param.Info.Detail, PatternUI.InterpolateHint, PatternUI.EditHint);
			return result;
		}

		static void OnValueKeyDown(Action interpolate, KeyEventArgs e)
		{
			if (e.Key == Key.I && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
				interpolate();
		}

		static UIElement MakeTarget(SynthModel synth,
			Param param, Param fxCount, int minFx, int row, int col)
		{
			var result = MakeHex(param, fxCount, minFx, row, col);
			var binding = Bind.To(param, nameof(Param.Value), new TargetFormatter(synth, param));
			result.SetBinding(FrameworkElement.ToolTipProperty, binding);
			return result;
		}

		static FrameworkElement MakeHex(Param param,
			Param fxCount, int minFx, int row, int col)
		{
			var result = Create.PatternCell<HexBox>(new(row, col));
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.OnParsed += (s, e) => Utility.FocusDown();
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(fxCount, minFx));
			return result;
		}
	}
}