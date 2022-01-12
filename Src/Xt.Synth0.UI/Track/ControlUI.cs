using System;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class ControlUI
	{
		public static event EventHandler Stop;
		public static event EventHandler Start;

		internal static UIElement Make(AppModel app)
		=> Create.ThemedGroup(app.Settings, app.Track.Sequencer.Control, MakeContent(app.Stream));

		static Button MakeButton(string content, Action execute)
		{
			var result = new Button();
			result.Content = content;
			result.Click += (s, e) => execute();
			return result;
		}

		static UIElement MakeContent(StreamModel stream)
		{
			var result = new DockPanel();
			result.LastChildFill = false;
			result.VerticalAlignment = VerticalAlignment.Bottom;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			var stop = MakeButton("Stop", () => Stop(null, EventArgs.Empty));
			var binding = Bind.To(stream, nameof(StreamModel.IsStopped), new NegateConverter());
			stop.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Add(stop, Dock.Right);
			var start = MakeButton("Start", () => Start(null, EventArgs.Empty));
			binding = Bind.To(stream, nameof(StreamModel.IsRunning), new NegateConverter());
			start.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Add(start, Dock.Right);
			result.SetResourceReference(Panel.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}
	}
}