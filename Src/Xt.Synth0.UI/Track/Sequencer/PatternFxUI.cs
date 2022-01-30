using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternFxUI
	{
		static void OnValueKeyDown(PatternFxElements elems, Action interpolate, KeyEventArgs e)
		{
			e.Handled = true;
			if (e.Key == Key.I && e.KeyboardDevice.Modifiers == ModifierKeys.Control) interpolate();
			else if (e.Key == Key.Down || e.Key == Key.Up) elems.RequestMoveValueFocus(false, e.Key == Key.Up);
			else e.Handled = false;
		}

		static void OnTargetKeyDown(Param target, PatternFxElements elems, Action fill, KeyEventArgs e)
		{
			e.Handled = true;
			if (e.Key == Key.F && e.KeyboardDevice.Modifiers == ModifierKeys.Control) fill();
			else if (e.Key == Key.Down || e.Key == Key.Up) elems.RequestMoveTargetFocus(false, e.Key == Key.Up);
			else if (e.Key == Key.Delete || e.Key == Key.OemPeriod)
			{
				target.Value = target.Info.Min;
				elems.RequestMoveTargetFocus(false, false);
			}
			else e.Handled = false;
		}

		internal static PatternFxElements Add(Grid grid, AppModel app,
			PatternFx fx, int minFx, int row, int col, Action fill, Action interpolate)
		{
			var result = new PatternFxElements();
			result.Target = grid.Add(MakeTarget(app, fx.Tgt, minFx, row, col, result, fill));
			result.Value = grid.Add(MakeValue(app, fx.Tgt, fx.Val, minFx, row, col + 1, result, interpolate));
			return result;
		}

		static HexBox MakeHex(AppModel app,
			Param param, int minFx, int row, int col)
		{
			var edit = app.Track.Seq.Edit;
			var result = Create.PatternCell<HexBox>(new(row, col));
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(Control.ForegroundProperty, Bind.EnableRow(app, row));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Fxs, minFx));
			return result;
		}

		static UIElement MakeTarget(AppModel app, Param target,
			int minFx, int row, int col, PatternFxElements elems, Action fill)
		{
			var seq = app.Track.Seq;
			var synth = app.Track.Synth;
			var result = MakeHex(app, target, minFx, row, col);
			result.OnParsed += (s, e) => elems.RequestMoveTargetFocus(true, false);
			var formatter = new TargetFormatter(synth, target);
			var binding = Bind.To(target, nameof(Param.Value), formatter);
			result.SetBinding(FrameworkElement.ToolTipProperty, binding);
			result.KeyDown += (s, e) => OnTargetKeyDown(target, elems, fill, e);
			binding = Bind.To(target, nameof(Param.Value), new PlaceholderConverter(0));
			result.SetBinding(HexBox.ShowPlaceholderProperty, binding);
			result.SetValue(ToolTipService.InitialShowDelayProperty, PatternUI.TooltipDelay);
			result.SetValue(ToolTipService.BetweenShowDelayProperty, PatternUI.BetweenTooltipDelay);
			return result;
		}

		static UIElement MakeValue(AppModel app, Param target, Param value,
			int minFx, int row, int col, PatternFxElements elems, Action interpolate)
		{
			var result = MakeHex(app, value, minFx, row, col);
			var binding = Bind.To(target, nameof(Param.Value), new PlaceholderConverter(0));
			result.SetBinding(HexBox.ShowPlaceholderProperty, binding);
			result.KeyDown += (s, e) => OnValueKeyDown(elems, interpolate, e);
			result.OnParsed += (s, e) => elems.RequestMoveValueFocus(true, false);
			result.SetValue(ToolTipService.InitialShowDelayProperty, PatternUI.TooltipDelay);
			result.SetValue(ToolTipService.BetweenShowDelayProperty, PatternUI.BetweenTooltipDelay);
			result.ToolTip = string.Join("\n", value.Info.Description, PatternUI.InterpolateHint, PatternUI.EditHint);
			return result;
		}
	}
}