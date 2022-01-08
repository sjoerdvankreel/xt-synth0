using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		const double HighlightedOpacity = 0.25;
		const double NotHighlightedOpacity = 0.0;
		static readonly object HighlightedOpacityBoxed = HighlightedOpacity;
		static readonly object NotHighlightedOpacityBoxed = NotHighlightedOpacity;

		internal const string EditHint = "Click + keyboard to edit";
		internal const string InterpolateHint = "Ctrl + I to interpolate";

		static void Fill(SequencerModel seq, int pattern, int fx)
		{
			var rows = seq.Pattern.Rows;
			int rowCount = TrackConstants.MaxRows;
			int start = pattern * rowCount;
			int end = start + rowCount - 1;
			for (int i = start; i <= end; i++)
				rows[i].Fx[fx].Target.Value = rows[start].Fx[fx].Target.Value;
		}

		static void Interpolate(SequencerModel seq,
			int pattern, Func<PatternRow, Param> selector)
		{
			var rows = seq.Pattern.Rows;
			int rowCount = TrackConstants.MaxRows;
			int start = pattern * rowCount;
			int end = start + rowCount - 1;
			int endValue = selector(rows[end]).Value;
			int startValue = selector(rows[start]).Value;
			float range = end - start;
			float rangeValue = endValue - startValue;
			for (int i = start; i <= end; i++)
				selector(rows[i]).Value = (int)(startValue + (i - start) / range * rangeValue);
		}

		internal static UIElement Make(AppModel model)
		{
			var result = new GroupBox();
			result.Content = MakeContent(model);
			result.SetBinding(HeaderedContentControl.HeaderProperty, BindHeader(model));
			return result;
		}

		static Border MakeHighlighter(Cell cell)
		{
			var result = Create.Element<Border>(cell);
			result.Opacity = 0.0;
			result.Focusable = false;
			result.Background = Brushes.Gray;
			return result;
		}

		static BindingBase BindHeader(AppModel model)
		{
			var pats = Bind.To(model.Track.Sequencer.Edit.Pats);
			var active = Bind.To(model.Track.Sequencer.Edit.Edit);
			var row = Bind.To(model.Stream, nameof(StreamModel.CurrentRow));
			var running = Bind.To(model.Stream, nameof(StreamModel.IsRunning));
			return Bind.To(new PatternFormatter(), running, pats, active, row);
		}

		static BindingBase BindSelector(AppModel model, UIElement[] patterns)
		{
			var active = Bind.To(model.Track.Sequencer.Edit.Edit);
			var row = Bind.To(model.Stream, nameof(StreamModel.CurrentRow));
			var running = Bind.To(model.Stream, nameof(StreamModel.IsRunning));
			return Bind.To(new PatternSelector(patterns), running, active, row);
		}

		static UIElement MakeContent(AppModel model)
		{
			var result = new ContentControl();
			var highlighters = new List<Border>();
			var patterns = new UIElement[TrackConstants.MaxPatterns];
			for (int p = 0; p < patterns.Length; p++)
			{
				var pattern = MakePattern(model, p);
				patterns[p] = pattern.Pattern;
				highlighters.AddRange(pattern.Highlighters);
			}
			var binding = Bind.To(model.Stream, nameof(StreamModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			result.SetBinding(ContentControl.ContentProperty, BindSelector(model, patterns));
			var dispatcher = Application.Current?.Dispatcher;
			Action<string> handler = property => OnHighlighterPropertyChanged(model.Stream, highlighters, property);
			model.Stream.PropertyChanged += (s, e) => dispatcher.BeginInvoke(handler, DispatcherPriority.Background, e.PropertyName);
			return result;
		}

		static (UIElement Pattern, IList<Border> Highlighters) MakePattern(AppModel model, int pattern)
		{
			var fx = TrackConstants.MaxFxs;
			var keys = TrackConstants.MaxKeys;
			var rows = TrackConstants.MaxRows;
			int cols = keys * 5 + 1 + fx * 3;
			var highlighters = new List<Border>();
			var sequencer = model.Track.Sequencer;
			var offset = pattern * rows;
			var result = Create.Grid(rows, cols);
			for (int r = 0; r < rows; r++)
			{
				var highlighter = MakeHighlighter(new Cell(r, 0, 1, cols));
				result.Add(highlighter);
				highlighters.Add(highlighter);
				AddRow(result, model, pattern, sequencer.Pattern.Rows[offset + r], r);
			}
			return (result, highlighters);
		}

		static void AddRow(Grid grid, AppModel app, int pattern, PatternRow row, int r)
		{
			var edit = app.Track.Sequencer.Edit;
			int divCol = TrackConstants.MaxKeys * 5;
			AddKeys(grid, app, pattern, row, r);
			grid.Add(Create.Divider(new(r, divCol), edit.Fxs, 1));
			AddFx(grid, app, pattern, row, r);
		}

		static void AddKeys(Grid grid, AppModel app, int pattern, PatternRow row, int r)
		{
			var seq = app.Track.Sequencer;
			for (int k = 0; k < TrackConstants.MaxKeys; k++)
			{
				int kLocal = k;
				Action interpolate = () => Interpolate(seq, pattern, r => r.Keys[kLocal].Amp);
				PatternKeyUI.Add(grid, row.Keys[k], seq.Edit, k + 1, r, k * 5, interpolate);
				grid.Add(Create.Divider(new(r, k * 5 + 4), seq.Edit.Keys, k + 2));
			}
		}

		static void AddFx(Grid grid, AppModel app, int pattern, PatternRow row, int r)
		{
			var seq = app.Track.Sequencer;
			int startCol = TrackConstants.MaxKeys * 5 + 1;
			for (int f = 0; f < TrackConstants.MaxFxs; f++)
			{
				int fLocal = f;
				Action fill = () => Fill(seq, pattern, fLocal);
				Action interpolate = () => Interpolate(seq, pattern, r => r.Fx[fLocal].Value);
				PatternFxUI.Add(grid, app.Track, row.Fx[f], f + 1, r, startCol + f * 3, fill, interpolate);
				grid.Add(Create.Divider(new(r, startCol + f * 3 + 2), seq.Edit.Fxs, f + 2));
			}
		}

		static void OnHighlighterPropertyChanged(
			StreamModel model, IList<Border> highlighters, string property)
		{
			if (property != nameof(model.IsRunning) && property != nameof(model.CurrentRow)) return;
			for (int i = 0; i < highlighters.Count; i++)
			{
				bool highlighted = model.IsRunning && i == model.CurrentRow;
				object opacity = highlighted ? HighlightedOpacityBoxed : NotHighlightedOpacityBoxed;
				highlighters[i].SetValue(UIElement.OpacityProperty, opacity);
			}
		}
	}
}