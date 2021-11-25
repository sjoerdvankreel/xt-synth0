using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		static readonly Key[] NoteKeys = new[]
		{
			Key.OemPeriod, Key.Space,
			Key.Q, Key.D2, Key.W, Key.D3, Key.E, Key.R,
			Key.D5, Key.T, Key.D6, Key.Y, Key.D7, Key.U
		};

		static T MakeCell<T>(Cell cell)
			where T: FrameworkElement, new()
		{
			var result = UI.MakeElement<T>(cell);
			result.Focusable = true;
			result.MouseLeftButtonDown += (s, e) => result.Focus();
			return result;
		}

		static void FocusNext(FocusNavigationDirection direction)
		{
			if (Keyboard.FocusedElement is UIElement e)
				e.MoveFocus(new(direction));
		}

		internal static UIElement Make(PatternModel model, string name, int offset,
			int count, Cell cell)
		{
			var result = UI.MakeElement<GroupBox>(cell);
			result.Header = name;
			result.Content = MakeContent(model, offset, count);
			return result;
		}

		static UIElement MakeContent(PatternModel model, int offset, int count)
		{
			var result = UI.MakeGrid(PatternModel.Length, 4);
			for (int n = 0; n < count; n++)
				AddRow(result, model.Rows[offset + n], n);
			return result;
		}

		static void AddRow(Grid grid, RowModel model, int row)
		{
			grid.Children.Add(MakeNote(model.Note, row));
			grid.Children.Add(MakeOct(model.Oct, row));
			grid.Children.Add(MakeAmp(model.Amp, row));
		}

		static UIElement MakeNote(Param param, int row)
		{
			var result = MakeCell<TextBlock>(new(row, 0));
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
			var direction = note >= (int)RowNote.C
				? FocusNavigationDirection.Next
				: FocusNavigationDirection.Down;
			FocusNext(direction);
		}

		static UIElement MakeOct(Param param, int row)
		{
			var result = MakeCell<TextBlock>(new(row, 1));
			result.TextInput += (s, e) => OnOctTextInput(param, e);
			result.SetBinding(TextBlock.TextProperty, UI.Bind(param));
			return result;
		}

		static void OnOctTextInput(Param param, TextCompositionEventArgs e)
		{
			int value = e.Text.FirstOrDefault() - '0';
			if (value < param.Info.Min || value > param.Info.Max) return;
			param.Value = value;
			e.Handled = true;
			FocusNext(FocusNavigationDirection.Next);
		}

		static UIElement MakeAmp(Param amp, int row)
		{
			var result = MakeCell<AmpBox>(new(row, 2));
			result.Minimum = amp.Info.Min;
			result.Maximum = amp.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, UI.Bind(amp));
			result.OnParsed += (s, e) => FocusNext(FocusNavigationDirection.Next);
			return result;
		}
	}
}