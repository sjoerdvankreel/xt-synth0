using System;
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
		const string NoteEditHint = $"Click + ./space/Z-M/Q-E to edit";

		static (int Oct, PatternNote Note) KeyToAction(Key key) => key switch
		{
			Key.Delete => (-1, PatternNote.None),
			Key.OemPeriod => (-1, PatternNote.None),
			Key.Space => (-1, PatternNote.Off),
			Key.Z => (0, PatternNote.C),
			Key.S => (0, PatternNote.CSharp),
			Key.X => (0, PatternNote.D),
			Key.D => (0, PatternNote.DSharp),
			Key.C => (0, PatternNote.E),
			Key.V => (0, PatternNote.F),
			Key.G => (0, PatternNote.FSharp),
			Key.B => (0, PatternNote.G),
			Key.H => (0, PatternNote.GSharp),
			Key.N => (0, PatternNote.A),
			Key.J => (0, PatternNote.ASharp),
			Key.M => (0, PatternNote.B),
			Key.Q => (1, PatternNote.C),
			Key.D2 => (1, PatternNote.CSharp),
			Key.W => (1, PatternNote.D),
			Key.D3 => (1, PatternNote.DSharp),
			Key.E => (1, PatternNote.E),
			Key.R => (1, PatternNote.F),
			Key.D5 => (1, PatternNote.FSharp),
			Key.T => (1, PatternNote.G),
			Key.D6 => (1, PatternNote.GSharp),
			Key.Y => (1, PatternNote.A),
			Key.D7 => (1, PatternNote.ASharp),
			Key.U => (1, PatternNote.B),
			Key.I => (2, PatternNote.C),
			Key.D9 => (2, PatternNote.CSharp),
			Key.O => (2, PatternNote.D),
			Key.D0 => (2, PatternNote.DSharp),
			Key.P => (2, PatternNote.E),
			_ => (-2, PatternNote.None)
		};

		static void OnOctTextInput(Param step, Param param, TextCompositionEventArgs e)
		{
			int value = e.Text.FirstOrDefault() - '0';
			if (value < param.Info.Min || value > param.Info.Max) return;
			param.Value = value;
			Utility.FocusDown(step.Value);
			e.Handled = true;
		}

		static void OnAmpKeyDown(Param step, Param amp, Action interpolate, KeyEventArgs e)
		{
			if (e.Key == Key.Delete || e.Key == Key.OemPeriod)
			{
				amp.Value = amp.Info.Max;
				Utility.FocusDown(step.Value);
			}
			if (e.Key == Key.I && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
				interpolate();
		}

		static void OnNoteKeyDown(EditModel edit, Param keyNote, Param keyOct, KeyEventArgs e)
		{
			var action = KeyToAction(e.Key);
			if (action.Oct == -2) return;
			e.Handled = true;
			if (action.Note == PatternNote.Off) keyNote.Value = (int)PatternNote.Off;
			if (action.Note == PatternNote.None) keyNote.Value = (int)PatternNote.None;
			keyNote.Value = (int)action.Note;
			keyOct.Value = Math.Min(9, edit.Oct.Value + action.Oct);
			Utility.FocusDown(edit.Step.Value);
		}

		internal static void Add(Grid grid, AppModel app,
			PatternKey key, int minKeys, int row, int col, Action interpolate)
		{
			var edit = app.Track.Seq.Edit;
			grid.Add(MakeNote(app, key.Note, key.Oct, minKeys, row, col));
			grid.Add(MakeOct(app, key, minKeys, row, col + 1));
			grid.Add(Create.Divider(new(row, col + 2), edit.Keys, minKeys));
			grid.Add(MakeAmp(app, key, minKeys, row, col + 3, interpolate));
		}

		static UIElement MakeNote(AppModel app, Param keyNote,
			Param keyOct, int minKeys, int row, int col)
		{
			var edit = app.Track.Seq.Edit;
			var result = Create.PatternCell<TextBlock>(new(row, col));
			result.ToolTip = string.Join("\n", keyNote.Info.Description, NoteEditHint);
			result.SetBinding(TextBlock.TextProperty, Bind.Format(keyNote));
			result.SetBinding(TextBlock.ForegroundProperty, Bind.EnableRow(app, row));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Keys, minKeys));
			result.KeyDown += (s, e) => OnNoteKeyDown(edit, keyNote, keyOct, e);
			return result;
		}

		static UIElement MakeOct(AppModel app, PatternKey key, int minKeys, int row, int col)
		{
			var edit = app.Track.Seq.Edit;
			var result = Create.PatternCell<TextBlock>(new(row, col));
			result.TextInput += (s, e) => OnOctTextInput(edit.Step, key.Oct, e);
			var binding = Bind.To(key.Note, key.Oct, new OctFormatter(key));
			result.SetBinding(TextBlock.TextProperty, binding);
			result.ToolTip = string.Join("\n", key.Oct.Info.Description, PatternUI.EditHint);
			result.SetBinding(TextBlock.ForegroundProperty, Bind.EnableRow(app, row));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Keys, minKeys));
			return result;
		}

		static UIElement MakeAmp(AppModel app, PatternKey key, int minKeys, int row, int col, Action interpolate)
		{
			var edit = app.Track.Seq.Edit;
			var result = Create.PatternCell<HexBox>(new(row, col));
			result.Minimum = key.Amp.Info.Min;
			result.Maximum = key.Amp.Info.Max;
			result.OnParsed += (s, e) => Utility.FocusDown(edit.Step.Value);
			result.KeyDown += (s, e) => OnAmpKeyDown(edit.Step, key.Amp, interpolate, e);
			result.SetBinding(RangeBase.ValueProperty, Bind.To(key.Amp));
			result.SetBinding(Control.ForegroundProperty, Bind.EnableRow(app, row));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(edit.Keys, minKeys));
			result.ToolTip = string.Join("\n", key.Amp.Info.Description, PatternUI.InterpolateHint, PatternUI.EditHint);
			var binding = Bind.To(key.Note, nameof(key.Note.Value), new Formatter<int>(v => v == (int)PatternNote.Off ? "=" : "."));
			result.SetBinding(HexBox.PlaceholderProperty, binding);
			binding = Bind.To(key.Note, nameof(key.Note.Value), new PlaceholderConverter((int)PatternNote.None, (int)PatternNote.Off));
			result.SetBinding(HexBox.ShowPlaceholderProperty, binding);
			return result;
		}
	}
}