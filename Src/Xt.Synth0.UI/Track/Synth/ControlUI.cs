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

		internal static UIElement Make(StreamModel model)
		=> Create.Group("Control", MakeContent(model));

		static Button MakeButton(string content, Action execute)
		{
			var result = new Button();
			result.Content = content;
			result.Click += (s, e) => execute();
			return result;
		}

		static UIElement MakeContent(StreamModel model)
		{
			var result = new DockPanel();
			result.LastChildFill = false;
			result.VerticalAlignment = VerticalAlignment.Bottom;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Panel.BackgroundProperty, "BackgroundParamKey");
			var stop = MakeButton("Stop", () => Stop(null, EventArgs.Empty));
			var binding = Bind.To(model, nameof(StreamModel.IsStopped), new NegateConverter());
			stop.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Add(stop, Dock.Right);
			var start = MakeButton("Start", () => Start(null, EventArgs.Empty));
			binding = Bind.To(model, nameof(StreamModel.IsRunning), new NegateConverter());
			start.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Add(start, Dock.Right);
			return result;
		}
	}
}