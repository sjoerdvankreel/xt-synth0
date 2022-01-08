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

		internal static void Add(Grid grid, TrackModel track, PatternFx fx,
			int minFx, int row, int col, Action fill, Action interpolate)
		{
			grid.Add(MakeTarget(track, fx.Target, minFx, row, col, fill));
			grid.Add(MakeValue(track.Sequencer.Edit, fx.Value, minFx, row, col + 1, interpolate));
		}

		static UIElement MakeValue(EditModel edit, Param param,
			int minFx, int row, int col, Action interpolate)
		{
			var result = MakeHex(edit, param, minFx, row, col);
			result.ToolTip = string.Join("\n", param.Info.Name,
				PatternUI.InterpolateHint, PatternUI.EditHint);
			result.KeyDown += (s, e) => OnValueKeyDown(interpolate, e);
			return result;
		}

		static UIElement MakeTarget(TrackModel track,
			Param param, int minFx, int row, int col, Action fill)
		{
			var result = MakeHex(track.Sequencer.Edit, param, minFx, row, col);
			var binding = Bind.To(param, nameof(Param.Value), new TargetFormatter(track.Synth, param));
			result.SetBinding(FrameworkElement.ToolTipProperty, binding);
			result.KeyDown += (s, e) => OnTargetKeyDown(fill, e);
			return result;
		}

		static FrameworkElement MakeHex(EditModel edit,
			Param param, int minFx, int row, int col)
		{
			var result = Create.PatternCell<HexBox>(new(row, col));
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.OnParsed += (s, e) => Utility.FocusDown();
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Fxs, minFx));
			result.SetBinding(Control.ForegroundProperty, Bind.EnableRow(edit.Rows, row));
			return result;
		}
	}
}