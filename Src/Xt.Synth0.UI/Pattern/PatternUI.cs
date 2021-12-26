using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal const string EditHint = "Click + keyboard to edit";
		internal const string InterpolateHint = "Ctrl + I to interpolate";

		static int GetHightlighterRow(AudioModel model, int pattern)
		=> IsHighlighted(model, pattern, out var row) ? row : 0;
		static Visibility GetHightlighterVisibility(AudioModel model, int pattern)
		=> IsHighlighted(model, pattern, out var _) ? Visibility.Visible : Visibility.Collapsed;

		static void Fill(SequencerModel sequencer, int pattern, int fx)
		{
			var rows = sequencer.Pattern.Rows;
			int rowCount = PatternModel.PatternRows;
			int start = pattern * rowCount;
			int end = start + rowCount - 1;
			for (int i = start; i <= end; i++)
				rows[i].Fx[fx].Target.Value = rows[start].Fx[fx].Target.Value;
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

		static void Interpolate(SequencerModel sequencer,
			int pattern, Func<PatternRow, Param> selector)
		{
			var rows = sequencer.Pattern.Rows;
			int rowCount = PatternModel.PatternRows;
			int start = pattern * rowCount;
			int end = start + rowCount - 1;
			int endValue = selector(rows[end]).Value;
			int startValue = selector(rows[start]).Value;
			float range = end - start;
			float rangeValue = endValue - startValue;
			for (int i = start; i <= end; i++)
				selector(rows[i]).Value = (int)(startValue + (i - start) / range * rangeValue);
		}

		static void OnAudioPropertyChanged(Border highlighter, AudioModel model, int pattern)
		{
			highlighter.Visibility = GetHightlighterVisibility(model, pattern);
			highlighter.SetValue(Grid.RowProperty, GetHightlighterRow(model, pattern));
		}

		internal static UIElement Make(AppModel model)
		{
			var result = new GroupBox();
			result.Content = MakeContent(model);
			result.SetBinding(HeaderedContentControl.HeaderProperty, BindHeader(model));
			return result;
		}

		static BindingBase BindHeader(AppModel model)
		{
			var pats = Bind.To(model.Track.Sequencer.Edit.Pats);
			var active = Bind.To(model.Track.Sequencer.Edit.Act);
			var row = Bind.To(model.Audio, nameof(AudioModel.CurrentRow));
			var running = Bind.To(model.Audio, nameof(AudioModel.IsRunning));
			return Bind.To(new PatternFormatter(), running, pats, active, row);
		}

		static BindingBase BindSelector(AppModel model, UIElement[] patterns)
		{
			var active = Bind.To(model.Track.Sequencer.Edit.Act);
			var row = Bind.To(model.Audio, nameof(AudioModel.CurrentRow));
			var running = Bind.To(model.Audio, nameof(AudioModel.IsRunning));
			return Bind.To(new PatternSelector(patterns), running, active, row);
		}

		static UIElement MakeContent(AppModel model)
		{
			var result = new ContentControl();
			var patterns = new UIElement[PatternModel.PatternCount];
			for (int p = 0; p < patterns.Length; p++)
				patterns[p] = MakePattern(model, p);
			var binding = Bind.To(model.Audio, nameof(AudioModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			result.SetBinding(ContentControl.ContentProperty, BindSelector(model, patterns));
			return result;
		}

		static UIElement MakePattern(AppModel model, int pattern)
		{
			var sequencer = model.Track.Sequencer;
			var fx = PatternRow.MaxFxCount;
			var keys = PatternRow.MaxKeyCount;
			var rows = PatternModel.PatternRows;
			int cols = keys * 5 + 1 + fx * 3;
			var offset = pattern * PatternModel.PatternRows;
			var result = Create.Grid(rows, cols);
			for (int r = 0; r < rows; r++)
				AddRow(result, model.Track, pattern, sequencer.Pattern.Rows[offset + r], r);
			AddHightlighter(result, model.Audio, pattern, cols);
			return result;
		}

		static void AddRow(Grid grid, TrackModel track, int pattern, PatternRow row, int r)
		{
			var sequencer = track.Sequencer;
			int divCol = PatternRow.MaxKeyCount * 5;
			AddKeys(grid, sequencer, pattern, row, r);
			grid.Children.Add(Create.Divider(new(r, divCol), sequencer.Edit.Fx, 1));
			AddFx(grid, track, pattern, row, r);
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
			model.PropertyChanged += (s, e) => Application.Current?.Dispatcher.BeginInvoke(handler);
		}

		static void AddKeys(Grid grid, SequencerModel sequencer, int pattern, PatternRow row, int r)
		{
			for (int k = 0; k < PatternRow.MaxKeyCount; k++)
			{
				int kLocal = k;
				Action interpolate = () => Interpolate(sequencer, pattern, r => r.Keys[kLocal].Amp);
				PatternKeyUI.Add(grid, row.Keys[k], sequencer.Edit, k + 1, r, k * 5, interpolate);
				grid.Children.Add(Create.Divider(new(r, k * 5 + 4), sequencer.Edit.Keys, k + 2));
			}
		}

		static void AddFx(Grid grid, TrackModel track, int pattern, PatternRow row, int r)
		{
			var synth = track.Synth;
			var sequencer = track.Sequencer;
			var fx = track.Sequencer.Edit.Fx;
			int startCol = PatternRow.MaxKeyCount * 5 + 1;
			for (int f = 0; f < PatternRow.MaxFxCount; f++)
			{
				int fLocal = f;
				Action fill = () => Fill(sequencer, pattern, fLocal);
				Action interpolate = () => Interpolate(sequencer, pattern, r => r.Fx[fLocal].Value);
				PatternFxUI.Add(grid, synth, row.Fx[f], sequencer.Edit.Fx, f + 1, r, startCol + f * 3, fill, interpolate);
				grid.Children.Add(Create.Divider(new(r, startCol + f * 3 + 2), fx, f + 2));
			}
		}
	}
}