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
		static void OnTargetKeyDown(Action fill, KeyEventArgs e)
		{
			if (e.Key == Key.F && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
				fill();
		}

		static void OnValueKeyDown(Action interpolate, KeyEventArgs e)
		{
			if (e.Key == Key.I && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
				interpolate();
		}

		internal static void Add(Grid grid, AppModel app, PatternFx fx,
			int minFx, int row, int col, Action fill, Action interpolate)
		{
			grid.Add(MakeTarget(app, fx.Tgt, minFx, row, col, fill));
			grid.Add(MakeValue(app, fx.Tgt, fx.Val, minFx, row, col + 1, interpolate));
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
			int minFx, int row, int col, Action fill)
		{
			var synth = app.Track.Synth;
			var result = MakeHex(app, target, minFx, row, col);
			result.OnParsed += (s, e) => Utility.FocusRight();
			var formatter = new TargetFormatter(synth, target);
			var binding = Bind.To(target, nameof(Param.Value), formatter);
			result.SetBinding(FrameworkElement.ToolTipProperty, binding);
			result.KeyDown += (s, e) => OnTargetKeyDown(fill, e);
			binding = Bind.To(target, nameof(Param.Value), new PlaceholderConverter(0));
			result.SetBinding(HexBox.ShowPlaceholderProperty, binding);
			return result;
		}

		static UIElement MakeValue(AppModel app, Param target, Param value,
			int minFx, int row, int col, Action interpolate)
		{
			var result = MakeHex(app, value, minFx, row, col);
			result.OnParsed += (s, e) => Utility.FocusDownLeft();
			result.ToolTip = string.Join("\n", value.Info.Description,
				PatternUI.InterpolateHint, PatternUI.EditHint);
			result.KeyDown += (s, e) => OnValueKeyDown(interpolate, e); 
			var binding = Bind.To(target, nameof(Param.Value), new PlaceholderConverter(0));
			result.SetBinding(HexBox.ShowPlaceholderProperty, binding);
			return result;
		}
	}
}