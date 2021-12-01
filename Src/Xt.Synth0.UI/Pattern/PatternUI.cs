using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal const string EditHint = "Click + keyboard to edit";

		internal static UIElement Make(
			SynthModel synth, AudioModel audio)
		{
			var track = synth.Track;
			var result = new GroupBox();
			result.Content = MakeContent(synth, audio);
			var binding = UI.Format(track.Edit, track.Pats, new PatternFormatter());
			result.SetBinding(HeaderedContentControl.HeaderProperty, binding);
			return result;
		}

		static UIElement MakeContent(
			SynthModel synth, AudioModel audio)
		{
			var result = new ContentControl();
			var patterns = new UIElement[PatternModel.PatternCount];
			for (int p = 0; p < patterns.Length; p++)
				patterns[p] = MakePattern(synth, p);
			var binding = UI.Bind(synth.Track.Edit);
			binding.Converter = new PatternSelector(patterns);
			result.SetBinding(ContentControl.ContentProperty, binding);
			binding = UI.Bind(audio, nameof(audio.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement MakePattern(SynthModel model, int index)
		{
			var track = model.Track;
			var pattern = model.Pattern;
			var rows = PatternModel.PatternRows;
			var offset = index * PatternModel.PatternRows;
			var result = UI.MakeGrid(rows, 20);
			for (int r = 0; r < rows; r++)
			{
				PatternKeyUI.Add(result, pattern.Rows[offset + r].Key1, track, 1, r, 0);
				result.Children.Add(UI.MakeDivider(new(r, 4), track.Keys, 2));
				PatternKeyUI.Add(result, pattern.Rows[offset + r].Key2, track, 2, r, 5);
				result.Children.Add(UI.MakeDivider(new(r, 9), track.Keys, 3));
				PatternKeyUI.Add(result, pattern.Rows[offset + r].Key3, track, 3, r, 10);
				result.Children.Add(UI.MakeDivider(new(r, 14), track.Fx, 1));
				PatternFxUI.Add(result, pattern.Rows[offset + r].Fx1, track.Fx, 1, r, 15);
				result.Children.Add(UI.MakeDivider(new(r, 17), track.Fx, 2));
				PatternFxUI.Add(result, pattern.Rows[offset + r].Fx2, track.Fx, 2, r, 18);
			}
			return result;
		}
	}
}