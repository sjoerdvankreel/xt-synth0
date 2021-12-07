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
			var rows = PatternModel.PatternRows;
			var offset = index * PatternModel.PatternRows;
			var result = Create.Grid(rows, 20);
			for (int r = 0; r < rows; r++)
			{
				PatternKeyUI.Add(result, pattern.Rows[offset + r].Keys[0], track, 1, r, 0);
				result.Children.Add(Create.Divider(new(r, 4), track.Keys, 2));
				PatternKeyUI.Add(result, pattern.Rows[offset + r].Keys[1], track, 2, r, 5);
				result.Children.Add(Create.Divider(new(r, 9), track.Keys, 3));
				PatternKeyUI.Add(result, pattern.Rows[offset + r].Keys[2], track, 3, r, 10);
				result.Children.Add(Create.Divider(new(r, 14), track.Fx, 1));
				PatternFxUI.Add(result, pattern.Rows[offset + r].Fx[0], track.Fx, 1, r, 15);
				result.Children.Add(Create.Divider(new(r, 17), track.Fx, 2));
				PatternFxUI.Add(result, pattern.Rows[offset + r].Fx[1], track.Fx, 2, r, 18);
			}
			return result;
		}
	}
}