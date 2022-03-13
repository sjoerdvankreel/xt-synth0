using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal const int TooltipDelay = 1000;
		internal const int BetweenTooltipDelay = 0;

		const double HighlightedOpacity = 0.25;
		const double NotHighlightedOpacity = 0.0;
		static readonly object HighlightedOpacityBoxed = HighlightedOpacity;
		static readonly object NotHighlightedOpacityBoxed = NotHighlightedOpacity;

		internal const string EditHint = "Click + keyboard to edit";
		internal const string InterpolateHint = "Ctrl + I to interpolate";

		static void Fill(SequencerModel seq, int pattern, int fx)
		{
			var rows = seq.Pattern.Rows;
			int rowCount = SequencerConfig.MaxRows;
			int start = pattern * rowCount;
			int end = start + rowCount - 1;
			for (int i = start; i <= end; i++)
				rows[i].Fx[fx].Target.Value = rows[start].Fx[fx].Target.Value;
		}

		static void Interpolate(SequencerModel seq,
			int pattern, Func<PatternRowModel, Param> selector)
		{
			var rows = seq.Pattern.Rows;
			int rowCount = SequencerConfig.MaxRows;
			int start = pattern * rowCount;
			int end = start + rowCount - 1;
			int endValue = selector(rows[end]).Value;
			int startValue = selector(rows[start]).Value;
			float range = end - start;
			float rangeValue = endValue - startValue;
			for (int i = start; i <= end; i++)
				selector(rows[i]).Value = (int)(startValue + (i - start) / range * rangeValue);
		}

		internal static UIElement Make(AppModel app)
		{
			var pattern = app.Track.Seq.Pattern;
			var result = Create.ThemedGroup(app.Settings, pattern, MakeContent(app));
			result.SetBinding(HeaderedContentControl.HeaderProperty, BindHeader(app));
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

		static BindingBase BindHeader(AppModel app)
		{
			var edit = app.Track.Seq.Edit;
			var pats = Bind.To(edit.Patterns);
			var active = Bind.To(edit.Edit);
			var header = app.Track.Seq.Pattern.Name;
			var row = Bind.To(app.Stream, nameof(StreamModel.CurrentRow));
			var running = Bind.To(app.Stream, nameof(StreamModel.IsStopped), new NegateConverter());
			return Bind.To(new PatternFormatter(header), running, pats, active, row);
		}

		static BindingBase BindSelector(AppModel app, UIElement[] patterns)
		{
			var active = Bind.To(app.Track.Seq.Edit.Edit);
			var row = Bind.To(app.Stream, nameof(StreamModel.CurrentRow));
			var running = Bind.To(app.Stream, nameof(StreamModel.IsStopped), new NegateConverter());
			return Bind.To(new PatternSelector(patterns), running, active, row);
		}

		static UIElement MakeContent(AppModel app)
		{
			var result = new ContentControl();
			var highlighters = new List<Border>();
			var patterns = new UIElement[SequencerConfig.MaxPatterns];
			for (int p = 0; p < patterns.Length; p++)
			{
				var pattern = MakePattern(app, p);
				patterns[p] = pattern.Pattern;
				highlighters.AddRange(pattern.Highlighters);
			}
			var edit = app.Track.Seq.Edit;
			var dispatcher = Application.Current?.Dispatcher;
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsStopped));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			result.SetBinding(ContentControl.ContentProperty, BindSelector(app, patterns));
			PropertyChangedEventHandler editHandler = (s, e) => UpdateHighlighters(app.Stream, edit, highlighters);
			edit.Lpb.PropertyChanged += editHandler;
			edit.Rows.PropertyChanged += editHandler;
			Action<string> streamHandler = property => OnStreamPropertyChanged(app.Stream, edit, highlighters, property);
			app.Stream.PropertyChanged += (s, e) => dispatcher.BeginInvoke(streamHandler, DispatcherPriority.Background, e.PropertyName);
			UpdateHighlighters(app.Stream, edit, highlighters);
			return result;
		}

		static (UIElement Pattern, IList<Border> Highlighters) MakePattern(AppModel app, int pattern)
		{
			var seq = app.Track.Seq;
			var fx = SequencerConfig.MaxFxs;
			var keys = SequencerConfig.MaxKeys;
			var rows = SequencerConfig.MaxRows;
			int cols = keys * 5 + 1 + fx * 3;
			var highlighters = new List<Border>();
			var offset = pattern * rows;
			var result = Create.Grid(rows, cols);
			var allElements = new List<PatternRowElements>();
			for (int r = 0; r < rows; r++)
			{
				var highlighter = MakeHighlighter(new Cell(r, 0, 1, cols));
				result.Add(highlighter);
				highlighters.Add(highlighter);
				var rowElements = AddRow(result, app, pattern, seq.Pattern.Rows[offset + r], r);
				allElements.Add(rowElements);
				ConnectFocusHandlers(seq.Edit, allElements, rowElements, r);
			}
			return (Create.ThemedContent(result), highlighters);
		}

		static PatternRowElements AddRow(Grid grid, AppModel app, int pattern, PatternRowModel row, int r)
		{
			var edit = app.Track.Seq.Edit;
			int divCol = SequencerConfig.MaxKeys * 5;
			var result = new PatternRowElements();
			result.Keys = AddKeys(grid, app, pattern, row, r);
			grid.Add(Create.Divider(new(r, divCol), edit.Fxs, 1));
			result.Fx = AddFx(grid, app, pattern, row, r);
			return result;
		}

		static IList<PatternKeyElements> AddKeys(Grid grid, AppModel app, int pattern, PatternRowModel row, int r)
		{
			var seq = app.Track.Seq;
			var result = new List<PatternKeyElements>();
			for (int k = 0; k < SequencerConfig.MaxKeys; k++)
			{
				int kLocal = k;
				Action interpolate = () => Interpolate(seq, pattern, r => r.Keys[kLocal].Velocity);
				result.Add(PatternKeyUI.Add(grid, app, row.Keys[k], k + 1, r, k * 5, interpolate));
				grid.Add(Create.Divider(new(r, k * 5 + 4), seq.Edit.Keys, k + 2));
			}
			return result;
		}

		static IList<PatternFxElements> AddFx(Grid grid, AppModel app, int pattern, PatternRowModel row, int r)
		{
			var seq = app.Track.Seq;
			var result = new List<PatternFxElements>();
			int startCol = SequencerConfig.MaxKeys * 5 + 1;
			for (int f = 0; f < SequencerConfig.MaxFxs; f++)
			{
				int fLocal = f;
				Action fill = () => Fill(seq, pattern, fLocal);
				Action interpolate = () => Interpolate(seq, pattern, r => r.Fx[fLocal].Value);
				result.Add(PatternFxUI.Add(grid, app, row.Fx[f], f + 1, r, startCol + f * 3, fill, interpolate));
				grid.Add(Create.Divider(new(r, startCol + f * 3 + 2), seq.Edit.Fxs, f + 2));
			}
			return result;
		}

		static void OnStreamPropertyChanged(
			StreamModel stream, EditModel edit, IList<Border> highlighters, string property)
		{
			if (property == nameof(stream.IsStopped) ||
				property == nameof(stream.CurrentRow))
				UpdateHighlighters(stream, edit, highlighters);
		}

		static void UpdateHighlighters(StreamModel stream, EditModel edit, IList<Border> highlighters)
		{
			if (!stream.IsStopped)
			{
				for (int i = 0; i < highlighters.Count; i++)
					highlighters[i].SetValue(UIElement.OpacityProperty,
						i == stream.CurrentRow ? HighlightedOpacityBoxed : NotHighlightedOpacityBoxed);
				return;
			}
			int actual = 0;
			foreach (var highlighter in highlighters)
				highlighter.SetValue(UIElement.OpacityProperty, NotHighlightedOpacityBoxed);
			for (int row = 0; row < highlighters.Count;)
			{
				if (actual % edit.Lpb.Value == 0)
					highlighters[row].SetValue(UIElement.OpacityProperty, HighlightedOpacityBoxed);
				row++;
				actual++;
				if (row % SequencerConfig.MaxRows == edit.Rows.Value)
					row += SequencerConfig.MaxRows - edit.Rows.Value;
			}
		}

		static void ConnectFocusHandlers(EditModel edit,
			IList<PatternRowElements> allElements, PatternRowElements rowElements, int row)
		{
			Func<int, int, int> rem = (row, rows) => (row + rows) % rows;
			for (int i = 0; i < rowElements.Fx.Count; i++)
			{
				int iLocal = i;
				Func<int, int, PatternFxElements> cycle = (r, d)
				=> allElements[rem(row + d * edit.Step.Value, edit.Rows.Value)].Fx[iLocal];
				rowElements.Fx[i].MoveValueFocus += (s, e) =>
				{
					if (e.Parsed) Keyboard.Focus(cycle(row, e.Up ? -1 : 1).Target);
					else Keyboard.Focus(cycle(row, e.Up ? -1 : 1).Value);
				};
				rowElements.Fx[i].MoveTargetFocus += (s, e) =>
				{
					if (e.Parsed) Keyboard.Focus(allElements[row].Fx[iLocal].Value);
					else Keyboard.Focus(cycle(row, e.Up ? -1 : 1).Target);
				};
			}
			for (int i = 0; i < rowElements.Keys.Count; i++)
			{
				int iLocal = i;
				Func<int, int, PatternKeyElements> cycle = (r, d)
				=> allElements[rem(r + d * edit.Step.Value, edit.Rows.Value)].Keys[iLocal];
				rowElements.Keys[i].MoveOctFocus += (s, e) => Keyboard.Focus(cycle(row, e.Up ? -1 : 1).Oct);
				rowElements.Keys[i].MoveAmpFocus += (s, e) => Keyboard.Focus(cycle(row, e.Up ? -1 : 1).Amp);
				rowElements.Keys[i].MoveNoteFocus += (s, e) => Keyboard.Focus(cycle(row, e.Up ? -1 : 1).Note);
			}
		}
	}
}