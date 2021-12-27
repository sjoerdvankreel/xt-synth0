using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternKeyUI
	{
		const string NoteEditHint = $"Click + ./space/Q-U to edit";

		static readonly Key[] NoteKeys = new[]
		{
			Key.OemPeriod, Key.Space,
			Key.Q, Key.D2, Key.W, Key.D3, Key.E, Key.R,
			Key.D5, Key.T, Key.D6, Key.Y, Key.D7, Key.U
		};

		static void OnNoteKeyDown(Param param, KeyEventArgs e)
		{
			int note = new List<Key>(NoteKeys).IndexOf(e.Key);
			if (note < 0) return;
			e.Handled = true;
			param.Value = note;
			Utility.FocusDown();
		}

		static void OnOctTextInput(Param param, TextCompositionEventArgs e)
		{
			int value = e.Text.FirstOrDefault() - '0';
			if (value < param.Info.Min || value > param.Info.Max) return;
			param.Value = value;
			Utility.FocusDown();
			e.Handled = true;
		}

		static void OnAmpKeyDown(Action interpolate, KeyEventArgs e)
		{
			if (e.Key == Key.I && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
				interpolate();
		}

		internal static void Add(Grid grid, PatternKey model,
			EditModel edit, int minKeys, int row, int col, Action interpolate)
		{
			grid.Add(MakeNote(model.Note, edit.Keys, minKeys, row, col));
			grid.Add(MakeOct(model, edit.Keys, minKeys, row, col + 1));
			grid.Add(Create.Divider(new(row, col + 2), edit.Keys, minKeys));
			grid.Add(MakeAmp(model, edit.Keys, minKeys, row, col + 3, interpolate));
		}

		static UIElement MakeNote(Param param,
			Param keys, int minKeys, int row, int col)
		{
			var result = Create.PatternCell<TextBlock>(new(row, col));
			result.ToolTip = string.Join("\n", param.Info.Detail, NoteEditHint);
			result.SetBinding(TextBlock.TextProperty, Bind.Format(param));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(keys, minKeys));
			result.KeyDown += (s, e) => OnNoteKeyDown(param, e);
			return result;
		}

		static UIElement MakeOct(PatternKey model,
			Param keys, int minKeys, int row, int col)
		{
			var result = Create.PatternCell<TextBlock>(new(row, col));
			result.TextInput += (s, e) => OnOctTextInput(model.Oct, e);
			var binding = Bind.To(model.Note, model.Oct, new OctFormatter(model));
			result.SetBinding(TextBlock.TextProperty, binding);
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(keys, minKeys));
			result.ToolTip = string.Join("\n", model.Oct.Info.Detail, PatternUI.EditHint);
			return result;
		}

		static UIElement MakeAmp(PatternKey model,
			Param keys, int minKeys, int row, int col, Action interpolate)
		{
			var result = Create.PatternCell<AmpBox>(new(row, col));
			result.Minimum = model.Amp.Info.Min;
			result.Maximum = model.Amp.Info.Max;
			result.OnParsed += (s, e) => Utility.FocusDown();
			result.ToolTip = string.Join("\n", model.Amp.Info.Detail,
				PatternUI.InterpolateHint, PatternUI.EditHint);
			result.KeyDown += (s, e) => OnAmpKeyDown(interpolate, e);
			result.SetBinding(AmpBox.NoteProperty, Bind.To(model.Note));
			result.SetBinding(RangeBase.ValueProperty, Bind.To(model.Amp));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(keys, minKeys));
			return result;
		}
	}
}