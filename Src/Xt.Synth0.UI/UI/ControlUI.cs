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

		public static UIElement Make(AudioModel model)
		{
			var result = new GroupBox();
			result.Header = "Control";
			result.Content = MakeContent(model);
			return result;
		}

		static Button MakeButton(string content, Action execute)
		{
			var result = new Button();
			result.Content = content;
			result.Click += (s, e) => execute();
			return result;
		}

		static UIElement MakeContent(AudioModel model)
		{
			var result = new WrapPanel();
			var start = MakeButton("Start", () => Start(null, EventArgs.Empty));
			var binding = Bind.To(model, nameof(AudioModel.IsRunning), new NegateConverter());
			start.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Children.Add(start);
			var stop = MakeButton("Stop", () => Stop(null, EventArgs.Empty));
			binding = Bind.To(model, nameof(AudioModel.IsRunning));
			stop.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Children.Add(stop);
			return result;
		}
	}
}