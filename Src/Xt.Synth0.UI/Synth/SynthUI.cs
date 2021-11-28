using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(
			SynthModel synth, UIModel ui)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(MakeLeft(synth, ui));
			result.Children.Add(PatternUI.Make(synth, ui));
			return result;
		}

		static void AddDocked(
			Panel panel, UIElement element)
		{
			panel.Children.Add(element);
			element.SetValue(DockPanel.DockProperty, Dock.Top);
		}

		static UIElement MakeLeft(
			SynthModel synth, UIModel ui)
		{
			var result = new DockPanel();
			AddDocked(result, GroupUI.Make(synth, synth.Unit1, ui));
			AddDocked(result, GroupUI.Make(synth, synth.Unit2, ui));
			AddDocked(result, GroupUI.Make(synth, synth.Unit3, ui));
			AddDocked(result, GroupUI.Make(synth, synth.Amp, ui));
			AddDocked(result, GroupUI.Make(synth, synth.Track, ui));
			return result;
		}
	}
}