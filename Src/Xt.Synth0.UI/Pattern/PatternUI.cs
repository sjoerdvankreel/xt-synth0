using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal const string EditHint = "Click + keyboard to edit";

		static int GetHightlighterRow(AudioModel model, int pattern)
		=> IsHighlighted(model, pattern, out var row) ? row : 0;
		static Visibility GetHightlighterVisibility(AudioModel model, int pattern)
		=> IsHighlighted(model, pattern, out var _) ? Visibility.Visible : Visibility.Collapsed;

		internal static UIElement Make(AppModel model)
		{
			var track = model.Synth.Track;
			var result = new GroupBox();
			result.Content = MakeContent(model);
			var binding = Bind.To(track.Edit, track.Pats, new PatternFormatter());
			result.SetBinding(HeaderedContentControl.HeaderProperty, binding);
			return result;
		}

		static UIElement MakeContent(AppModel model)
		{
			var result = new ContentControl();
			var patterns = new UIElement[PatternModel.PatternCount];
			for (int p = 0; p < patterns.Length; p++)
				patterns[p] = MakePattern(model, p);
			var binding = Bind.To(model.Audio, nameof(AudioModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			var editBinding = Bind.To(model.Synth.Track.Edit);
			var rowBinding = Bind.To(model.Audio, nameof(AudioModel.CurrentRow));
			var runningBinding = Bind.To(model.Audio, nameof(AudioModel.IsRunning));
			var selectBinding = Bind.To(runningBinding, editBinding, rowBinding, new PatternSelector(patterns));
			result.SetBinding(ContentControl.ContentProperty, selectBinding);
			return result;
		}

		static UIElement MakePattern(AppModel model, int pattern)
		{
			var track = model.Synth.Track;
			var fx = PatternRow.MaxFxCount;
			var keys = PatternRow.MaxKeyCount;
			var rows = PatternModel.PatternRows;
			int cols = keys * 5 + 1 + fx * 3;
			var offset = pattern * PatternModel.PatternRows;
			var result = Create.Grid(rows, cols);
			for (int r = 0; r < rows; r++)
				AddRow(result, track, model.Synth.Pattern.Rows[offset + r], r);
			AddHightlighter(result, model.Audio, pattern, cols);
			return result;
		}

		static void AddRow(Grid grid, TrackModel track, PatternRow row, int r)
		{
			int divCol = PatternRow.MaxKeyCount * 5;
			AddKeys(grid, track, row, r);
			grid.Children.Add(Create.Divider(new(r, divCol), track.Fx, 1));
			AddFx(grid, track, row, r);
		}

		static void AddKeys(Grid grid, TrackModel track, PatternRow row, int r)
		{
			for (int k = 0; k < PatternRow.MaxKeyCount; k++)
			{
				PatternKeyUI.Add(grid, row.Keys[k], track, k + 1, r, k * 5);
				grid.Children.Add(Create.Divider(new(r, k * 5 + 4), track.Keys, k + 2));
			}
		}

		static void AddFx(Grid grid, TrackModel track, PatternRow row, int r)
		{
			int startCol = PatternRow.MaxKeyCount * 5 + 1;
			for (int f = 0; f < PatternRow.MaxFxCount; f++)
			{
				PatternFxUI.Add(grid, row.Fx[f], track.Fx, f + 1, r, startCol + f * 3);
				grid.Children.Add(Create.Divider(new(r, startCol + f * 3 + 2), track.Fx, f + 2));
			}
		}

		static bool IsHighlighted(AudioModel model, int pattern, out int row)
		{
			row = -1;
			if (!model.IsRunning) return false;
			int startRow = pattern * PatternModel.PatternRows;
			int endRow = startRow + PatternModel.PatternRows;
			if (model.CurrentRow < startRow || model.CurrentRow >= endRow) return false;
			row = model.CurrentRow - startRow;
			return true;
		}

		static void OnAudioPropertyChanged(Border highlighter, AudioModel model, int pattern)
		{
			highlighter.Visibility = GetHightlighterVisibility(model, pattern);
			highlighter.SetValue(Grid.RowProperty, GetHightlighterRow(model, pattern));
		}

		static void AddHightlighter(Grid grid, AudioModel model, int pattern, int cols)
		{
			var result = Create.Element<Border>(new Cell(0, 0, 1, cols));
			grid.Children.Add(result);
			result.Opacity = 0.25;
			result.Background = Brushes.Gray;
			result.Visibility = GetHightlighterVisibility(model, pattern);
			result.SetValue(Grid.RowProperty, GetHightlighterRow(model, pattern));
			Action handler = () => OnAudioPropertyChanged(result, model, pattern);
			model.PropertyChanged += (s, e) => Application.Current.Dispatcher.BeginInvoke(handler);
		}
	}
}