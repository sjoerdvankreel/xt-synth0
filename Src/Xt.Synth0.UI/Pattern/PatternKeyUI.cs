using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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

		internal static BindingBase FormatOct(PatternKey model)
		{
			var result = new MultiBinding();
			result.Bindings.Add(UI.Bind(model.Note));
			result.Bindings.Add(UI.Bind(model.Oct));
			result.Converter = new OctFormatter(model);
			return result;
		}

		internal static void Add(Grid grid, PatternKey model,
			TrackModel track, int minKeys, int row, int col)
		{
			grid.Children.Add(MakeNote(model.Note, track.Keys, minKeys, row, col));
			grid.Children.Add(MakeOct(model, track.Keys, minKeys, row, col + 1));
			grid.Children.Add(UI.MakeDivider(new(row, col + 2), track.Keys, minKeys));
			grid.Children.Add(MakeAmp(model, track.Keys, minKeys, row, col + 3));
		}

		static UIElement MakeNote(Param param,
			Param keys, int minKeys, int row, int col)
		{
			var result = UI.MakePatternCell<TextBlock>(new(row, col));
			result.ToolTip = string.Join("\n", param.Info.Detail, NoteEditHint);
			result.SetBinding(TextBlock.TextProperty, UI.Format(param));
			result.SetBinding(UIElement.VisibilityProperty, UI.Show(keys, minKeys));
			result.KeyDown += (s, e) => OnNoteKeyDown(param, e);
			return result;
		}

		static void OnNoteKeyDown(Param param, KeyEventArgs e)
		{
			int note = new List<Key>(NoteKeys).IndexOf(e.Key);
			if (note < 0) return;
			e.Handled = true;
			param.Value = note;
			var direction = note >= (int)PatternNote.C
				? FocusNavigationDirection.Next
				: FocusNavigationDirection.Down;
			UI.FocusNext(direction);
		}

		static UIElement MakeOct(PatternKey model,
			Param keys, int minKeys, int row, int col)
		{
			var result = UI.MakePatternCell<TextBlock>(new(row, col));
			result.TextInput += (s, e) => OnOctTextInput(model.Oct, e);
			result.SetBinding(TextBlock.TextProperty, FormatOct(model));
			result.SetBinding(UIElement.VisibilityProperty, UI.Show(keys, minKeys));
			result.ToolTip = string.Join("\n", model.Oct.Info.Detail, PatternUI.EditHint);
			return result;
		}

		static void OnOctTextInput(Param param, TextCompositionEventArgs e)
		{
			int value = e.Text.FirstOrDefault() - '0';
			if (value < param.Info.Min || value > param.Info.Max) return;
			param.Value = value;
			e.Handled = true;
			UI.FocusNext(FocusNavigationDirection.Next);
		}

		static UIElement MakeAmp(PatternKey model,
			Param keys, int minKeys, int row, int col)
		{
			var result = UI.MakePatternCell<AmpBox>(new(row, col));
			result.Minimum = model.Amp.Info.Min;
			result.Maximum = model.Amp.Info.Max;
			result.SetBinding(AmpBox.NoteProperty, UI.Bind(model.Note));
			result.SetBinding(RangeBase.ValueProperty, UI.Bind(model.Amp));
			result.SetBinding(UIElement.VisibilityProperty, UI.Show(keys, minKeys));
			result.OnParsed += (s, e) => UI.FocusNext(FocusNavigationDirection.Next);
			result.ToolTip = string.Join("\n", model.Amp.Info.Detail, PatternUI.EditHint);
			return result;
		}
	}
}