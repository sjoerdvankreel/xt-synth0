using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class PatternUI
	{
		const int CellMargin = 2;
		static readonly FontFamily Font = new FontFamily("Consolas");
		static readonly double CellWidth = new FormattedText("C",
			CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
			Font.GetTypefaces().First(), new TextBlock().FontSize,
			Brushes.Black, VisualTreeHelper.GetDpi(new TextBlock()).PixelsPerDip).Width;

		static int KeyToNote(Key key) => key switch
		{
			Key.Q => NoteModel.C,
			Key.D2 => NoteModel.CSharp,
			Key.W => NoteModel.D,
			Key.D3 => NoteModel.DSharp,
			Key.E => NoteModel.E,
			Key.R => NoteModel.F,
			Key.D5 => NoteModel.FSharp,
			Key.T => NoteModel.G,
			Key.D6 => NoteModel.GSharp,
			Key.Y => NoteModel.A,
			Key.D7 => NoteModel.ASharp,
			Key.U => NoteModel.B,
			Key.Space => RowModel.NoteOff,
			Key.OemPeriod => RowModel.NoNote,
			_ => -1
		};

		static string FormatOctave(object[] args)
		{
			int note = (int)args[0];
			if (note < NoteModel.NoteCount)
				return ((int)args[1]).ToString();
			return note == RowModel.NoteOff ? "=" : ".";
		}

		static string FormatAmp(object[] args)
		{
			int note = (int)args[0];
			if (note < NoteModel.NoteCount)
				return ((int)args[1]).ToString("X2");
			return note == RowModel.NoteOff ? "==" : "..";
		}

		static string FormatNote(int note) => note switch
		{
			RowModel.NoNote => ".",
			RowModel.NoteOff => "==",
			_ => UI.NoteNames[note]
		};

		internal static UIElement Make(PatternModel model, string name,
			int row, int column, int rowSpan = 1, int columnSpan = 1)
		{
			var result = UI.MakeElement<GroupBox>(row, column, rowSpan, columnSpan);
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}

		static UIElement MakeContent(PatternModel model)
		{
			var result = UI.MakeGrid(PatternModel.Length, 3);
			for (int n = 0; n < PatternModel.Length; n++)
				AddRow(result, model.Rows[n], n);
			return result;
		}

		static void AddRow(Grid grid, RowModel model, int row)
		{
			grid.Children.Add(MakeNote(model.Note, row));
			grid.Children.Add(MakeOctave(model.Note, model.Octave, row));
			grid.Children.Add(MakeAmp(model.Note, model.Amp, row));
		}

		static FrameworkElement MakeCell(int chars, int row, int column)
		{
			var result = UI.MakeElement<TextBlock>(row, column);
			result.Margin = new Thickness(CellMargin);
			result.Focusable = true;
			result.FontFamily = Font;
			result.Width = CellWidth * chars;
			result.MouseLeftButtonDown += (s, e) => result.Focus();
			return result;
		}

		static UIElement MakeNote(Param<int> note, int row)
		{
			var result = MakeCell(2, row, 0);
			var binding = Bind.To(note, FormatNote);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.KeyDown += (s, e) => OnNoteKeyDown(note, e);
			return result;
		}

		static void OnNoteKeyDown(Param<int> param, KeyEventArgs e)
		{
			int note = KeyToNote(e.Key);
			if (note != -1)
				param.Value = note;
		}

		static UIElement MakeOctave(Param<int> note, Param<int> octave, int row)
		{
			var result = MakeCell(1, row, 1);
			var noteBinding = Bind.To(note);
			var octaveBinding = Bind.To(octave);
			var binding = Bind.Of(FormatOctave, noteBinding, octaveBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.TextInput += (s, e) => OnOctaveTextInput(octave, e);
			return result;
		}

		static void OnOctaveTextInput(Param<int> param, TextCompositionEventArgs e)
		{
			var octave = e.Text.FirstOrDefault();
			if (octave >= '0' && octave <= '9')
				param.Value = octave - '0';
		}

		static UIElement MakeAmp(Param<int> note, Param<int> amp, int row)
		{
			var result = MakeCell(2, row, 2);
			var ampBinding = Bind.To(amp);
			var noteBinding = Bind.To(note);
			var binding = Bind.Of(FormatAmp, noteBinding, ampBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}
	}
}