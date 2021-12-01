using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(
			SynthModel synth, OptionsModel options, AudioModel audio)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(MakeLeft(synth, options, audio));
			result.Children.Add(PatternUI.Make(synth, audio));
			return result;
		}

		static void AddDocked(
			Panel panel, UIElement element)
		{
			panel.Children.Add(element);
			element.SetValue(DockPanel.DockProperty, Dock.Top);
		}

		static UIElement MakeLeft(
			SynthModel synth, OptionsModel options, AudioModel audio)
		{
			var result = new DockPanel();
			AddDocked(result, GroupUI.Make(synth, synth.Unit1, options, audio));
			AddDocked(result, GroupUI.Make(synth, synth.Unit2, options, audio));
			AddDocked(result, GroupUI.Make(synth, synth.Unit3, options, audio));
			AddDocked(result, GroupUI.Make(synth, synth.Amp, options, audio));
			AddDocked(result, GroupUI.Make(synth, synth.Track, options, audio));
			return result;
		}
	}
}