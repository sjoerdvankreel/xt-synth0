using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal const string EditHint = "Click + keyboard to edit";

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
				patterns[p] = MakePattern(model.Synth, p);
			var binding = Bind.To(model.Synth.Track.Edit);
			binding.Converter = new PatternSelector(patterns);
			result.SetBinding(ContentControl.ContentProperty, binding);
			binding = Bind.To(model.Audio, nameof(AudioModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement MakePattern(SynthModel model, int index)
		{
			var track = model.Track;
			var pattern = model.Pattern;
			var fx = PatternRow.MaxFxCount;
			var keys = PatternRow.MaxKeyCount;
			var rows = PatternModel.PatternRows;
			var offset = index * PatternModel.PatternRows;
			var result = Create.Grid(rows, keys * 5 + fx * 3);
			for (int r = 0; r < rows; r++)
				AddRow(result, track, pattern.Rows[offset + r], r);
			return result;
		}

		static void AddRow(Grid grid, TrackModel track, PatternRow row, int r)
		{
			AddKeys(grid, track, row, r);
			AddFx(grid, track, row, r);
		}

		static void AddKeys(Grid grid, TrackModel track, PatternRow row, int r)
		{
			for (int k = 0; k < PatternRow.MaxKeyCount; k++)
			{
				PatternKeyUI.Add(grid, row.Keys[k], track, k + 1, r, k * 5);
				grid.Children.Add(Create.Divider(new(r, k * 5 + 4), track.Keys, k + 1));
			}
		}

		static void AddFx(Grid grid, TrackModel track, PatternRow row, int r)
		{
			int startCol = PatternRow.MaxKeyCount * 5;
			for (int f = 0; f < PatternRow.MaxFxCount; f++)
			{
				PatternFxUI.Add(grid, row.Fx[f], track.Fx, f + 1, r, startCol + f * 3);
				grid.Children.Add(Create.Divider(new(r, startCol + f * 3 + 2), track.Fx, f + 2));
			}
		}
	}
}