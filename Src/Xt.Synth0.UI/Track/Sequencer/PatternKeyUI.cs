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

		internal static void Add(Grid grid, AppModel app,
			PatternKey key, int minKeys, int row, int col, Action interpolate)
		{
			var edit = app.Track.Sequencer.Edit;
			grid.Add(MakeNote(app, key.Note, minKeys, row, col));
			grid.Add(MakeOct(app, key, minKeys, row, col + 1));
			grid.Add(Create.Divider(new(row, col + 2), edit.Keys, minKeys));
			grid.Add(MakeAmp(app, key, minKeys, row, col + 3, interpolate));
		}

		static UIElement MakeNote(AppModel app, Param note, int minKeys, int row, int col)
		{
			var edit = app.Track.Sequencer.Edit;
			var result = Create.PatternCell<TextBlock>(new(row, col));
			result.ToolTip = string.Join("\n", note.Info.Name, NoteEditHint);
			var binding = Bind.EnableRow(TextBlock.ForegroundProperty, result, app, row);
			result.SetBinding(TextBlock.ForegroundProperty, binding);
			result.SetBinding(TextBlock.TextProperty, Bind.Format(note));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Keys, minKeys));
			result.KeyDown += (s, e) => OnNoteKeyDown(note, e);
			return result;
		}

		static UIElement MakeOct(AppModel app, PatternKey key, int minKeys, int row, int col)
		{
			var edit = app.Track.Sequencer.Edit;
			var result = Create.PatternCell<TextBlock>(new(row, col));
			result.TextInput += (s, e) => OnOctTextInput(key.Oct, e);
			var binding = Bind.To(key.Note, key.Oct, new OctFormatter(key));
			result.SetBinding(TextBlock.TextProperty, binding);
			result.ToolTip = string.Join("\n", key.Oct.Info.Name, PatternUI.EditHint);
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Keys, minKeys));
			binding = Bind.EnableRow(TextBlock.ForegroundProperty, result, app, row);
			result.SetBinding(TextBlock.ForegroundProperty, binding);
			return result;
		}

		static UIElement MakeAmp(AppModel app, PatternKey key, int minKeys, int row, int col, Action interpolate)
		{
			var edit = app.Track.Sequencer.Edit;
			var result = Create.PatternCell<AmpBox>(new(row, col));
			result.Minimum = key.Amp.Info.Min;
			result.Maximum = key.Amp.Info.Max;
			result.OnParsed += (s, e) => Utility.FocusDown();
			result.KeyDown += (s, e) => OnAmpKeyDown(interpolate, e);
			result.SetBinding(AmpBox.NoteProperty, Bind.To(key.Note));
			result.SetBinding(RangeBase.ValueProperty, Bind.To(key.Amp));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Keys, minKeys));
			result.ToolTip = string.Join("\n", key.Amp.Info.Name, PatternUI.InterpolateHint, PatternUI.EditHint);
			var binding = Bind.EnableRow(Control.ForegroundProperty, result, app, row);
			result.SetBinding(Control.ForegroundProperty, binding);
			return result;
		}
	}
}