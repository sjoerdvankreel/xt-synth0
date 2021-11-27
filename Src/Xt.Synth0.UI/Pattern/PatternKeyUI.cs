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

		internal static void Add(
			Grid grid, PatternKey model, int row, int col)
		{
			grid.Children.Add(MakeNote(model.Note, row, col));
			grid.Children.Add(MakeOct(model, row, col + 1));
			grid.Children.Add(UI.MakeDivider(new(row, col + 2)));
			grid.Children.Add(MakeAmp(model.Amp, row, col + 3));
		}

		static UIElement MakeNote(Param param, int row, int col)
		{
			var result = UI.MakePatternCell<TextBlock>(new(row, col));
			var binding = UI.Format(param);
			result.SetBinding(TextBlock.TextProperty, binding);
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

		static UIElement MakeOct(PatternKey model, int row, int col)
		{
			var result = UI.MakePatternCell<TextBlock>(new(row, col));
			result.TextInput += (s, e) => OnOctTextInput(model.Oct, e);
			result.SetBinding(TextBlock.TextProperty, FormatOct(model));
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

		static UIElement MakeAmp(Param amp, int row, int col)
		{
			var result = UI.MakePatternCell<Hex>(new(row, col));
			result.Minimum = amp.Info.Min;
			result.Maximum = amp.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, UI.Bind(amp));
			result.OnParsed += (s, e) => UI.FocusNext(FocusNavigationDirection.Next);
			return result;
		}
	}
}