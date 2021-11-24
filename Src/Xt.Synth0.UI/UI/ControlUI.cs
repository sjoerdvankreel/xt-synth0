using System;
using System.Windows;
using System.Windows.Controls;

namespace Xt.Synth0.UI
{
	public static class ControlUI
	{
		static event EventHandler _stopped;
		static event EventHandler _started;

		public static event EventHandler Stop;
		public static event EventHandler Start;
		public static void Stopped() => _stopped?.Invoke(null, EventArgs.Empty);
		public static void Started() => _started?.Invoke(null, EventArgs.Empty);

		public static UIElement Make()
		{
			var result = new GroupBox();
			result.Header = "Control";
			result.Content = MakeContent();
			return result;
		}

		public static UIElement MakeContent()
		{
			var result = new WrapPanel();
			var doStart = () => Start?.Invoke(null, EventArgs.Empty);
			var start = UI.MakeButton("Start", doStart);
			result.Children.Add(start);
			var doStop = () => Stop?.Invoke(null, EventArgs.Empty);
			var stop = UI.MakeButton("Stop", doStop);
			stop.IsEnabled = false;
			result.Children.Add(stop);
			_started += (s, e) => start.IsEnabled = false;
			_started += (s, e) => stop.IsEnabled = true;
			_stopped += (s, e) => start.IsEnabled = true;
			_stopped += (s, e) => stop.IsEnabled = false;
			return result;
		}
	}
}