﻿using System;
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

		static string FormatOct(object[] args)
		=> Format(args, o => o.ToString());
		static string FormatAmp1(object[] args)
		=> Format(args, a => (a & 0X0000000F).ToString("X"));
		static string FormatAmp0(object[] args)
		=> Format(args, a => ((a & 0X000000F0) >> 4).ToString("X"));
		static int ParseAmp(string text)
		=> int.TryParse(text.FirstOrDefault().ToString(), NumberStyles.HexNumber,
			CultureInfo.CurrentCulture, out int val) ? val : -1;

		static string FormatNote(int note) => note switch
		{
			RowModel.NoNote => ".",
			RowModel.NoteOff => "==",
			_ => UI.NoteNames[note]
		};

		static string Format(object[] args, Func<int, string> display)
		{
			int note = (int)args[0];
			if (note < NoteModel.NoteCount)
				return display((int)args[1]);
			return note == RowModel.NoteOff ? "=" : ".";
		}

		static void FocusNext(FocusNavigationDirection direction)
		{
			if (Keyboard.FocusedElement is UIElement e)
				e.MoveFocus(new TraversalRequest(direction));
		}

		internal static UIElement Make(PatternModel model, string name, int offset,
			int count, int row, int column, int rowSpan = 1, int columnSpan = 1)
		{
			var result = UI.MakeElement<GroupBox>(row, column, rowSpan, columnSpan);
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
			grid.Children.Add(MakeOct(model.Note, model.Oct, row));
			grid.Children.Add(MakeAmp0(model.Note, model.Amp, row));
			grid.Children.Add(MakeAmp1(model.Note, model.Amp, row));
		}

		static UIElement MakeNote(Param<int> note, int row)
		{
			var result = MakeCell(row, 0, 2, CellMargin, CellMargin);
			var binding = Bind.To(note, FormatNote);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.KeyDown += (s, e) => OnNoteKeyDown(note, e);
			return result;
		}

		static void OnNoteKeyDown(Param<int> param, KeyEventArgs e)
		{
			int note = KeyToNote(e.Key);
			if (note == -1) return;
			param.Value = note;
			e.Handled = true;
			FocusNext(note < NoteModel.NoteCount ? FocusNavigationDirection.Next : FocusNavigationDirection.Down);
		}

		static UIElement MakeOct(Param<int> note, Param<int> oct, int row)
		{
			var result = MakeCell(row, 1, 1, CellMargin, CellMargin);
			var noteBinding = Bind.To(note);
			var octBinding = Bind.To(oct);
			var binding = Bind.Of(FormatOct, noteBinding, octBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.TextInput += (s, e) => OnOctTextInput(oct, e);
			return result;
		}

		static void OnOctTextInput(Param<int> param, TextCompositionEventArgs e)
		{
			var oct = e.Text.FirstOrDefault();
			if (oct < '0' || oct > '9') return;
			param.Value = oct - '0';
			e.Handled = true;
			FocusNext(FocusNavigationDirection.Next);
		}

		static UIElement MakeAmp0(Param<int> note, Param<int> amp, int row)
		{
			var result = MakeCell(row, 2, 1, CellMargin, 0);
			var ampBinding = Bind.To(amp);
			var noteBinding = Bind.To(note);
			var binding = Bind.Of(FormatAmp0, noteBinding, ampBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.TextInput += (s, e) => OnAmp0TextInput(amp, e);
			return result;
		}

		static void OnAmp0TextInput(Param<int> param, TextCompositionEventArgs e)
		{
			var val = ParseAmp(e.Text);
			if (val == -1) return;
			param.Value = (val << 4) | (param.Value & 0x0000000F);
			e.Handled = true;
			FocusNext(FocusNavigationDirection.Next);
		}

		static UIElement MakeAmp1(Param<int> note, Param<int> amp, int row)
		{
			var result = MakeCell(row, 3, 1, 0, CellMargin);
			var ampBinding = Bind.To(amp);
			var noteBinding = Bind.To(note);
			var binding = Bind.Of(FormatAmp1, noteBinding, ampBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.TextInput += (s, e) => OnAmp1TextInput(amp, e);
			return result;
		}

		static void OnAmp1TextInput(Param<int> param, TextCompositionEventArgs e)
		{
			var val = ParseAmp(e.Text);
			if (val == -1) return;
			param.Value = (param.Value & 0x000000F0) | val;
			e.Handled = true;
			FocusNext(FocusNavigationDirection.Next);
		}

		static FrameworkElement MakeCell(int row, int column, int chars, int leftMargin, int rightMargin)
		{
			var result = UI.MakeElement<TextBlock>(row, column);
			result.Margin = new Thickness(leftMargin, CellMargin, rightMargin, CellMargin);
			result.Focusable = true;
			result.FontFamily = Font;
			result.Width = CellWidth * chars;
			result.MouseLeftButtonDown += (s, e) => result.Focus();
			return result;
		}
	}
}