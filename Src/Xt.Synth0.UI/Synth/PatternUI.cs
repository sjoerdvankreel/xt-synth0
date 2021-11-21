using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class PatternUI
	{
		static int CellMargin = 2;

		static string FormatAmp(object[] args) => (int)args[0] < 12
			? ((int)args[1]).ToString("X2") : FormatNote((int)args[0]);
		static string FormatOctave(object[] args) => (int)args[0] < 12
			? ((int)args[1]).ToString() : FormatNote((int)args[0]);

		static string FormatNote(int note)
		{
			switch (note)
			{
				case RowModel.NoNote: return ".";
				case RowModel.NoteOff: return "_";
				default: return UI.NoteNames[note];
			}
		}

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

		static UIElement MakeNote(Param<int> note, int row)
		{
			var result = UI.MakeElement<TextBlock>(row, 0);
			result.Margin = new Thickness(CellMargin);
			result.SetBinding(TextBlock.TextProperty, Bind.To(note, FormatNote));
			return result;
		}

		static UIElement MakeOctave(Param<int> note, Param<int> octave, int row)
		{
			var result = UI.MakeElement<TextBlock>(row, 1);
			result.Margin = new Thickness(CellMargin);
			var noteBinding = Bind.To(note);
			var octaveBinding = Bind.To(octave);
			var binding = Bind.Of(FormatOctave, noteBinding, octaveBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}

		static UIElement MakeAmp(Param<int> note, Param<int> amp, int row)
		{
			var result = UI.MakeElement<TextBlock>(row, 2);
			result.Margin = new Thickness(CellMargin);
			var ampBinding = Bind.To(amp);
			var noteBinding = Bind.To(note);
			var binding = Bind.Of(FormatAmp, noteBinding, ampBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}
	}
}