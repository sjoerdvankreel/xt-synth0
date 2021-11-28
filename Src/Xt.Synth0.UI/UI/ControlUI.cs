using System;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class ControlUI
	{
		public static UIElement Make(UIModel model)
		{
			var result = new GroupBox();
			result.Header = "Control";
			result.Content = MakeContent(model);
			return result;
		}

		public static UIElement MakeContent(UIModel model)
		{
			var result = new WrapPanel();
			var start = MakeButton("Start", model.RequestStart);
			var binding = UI.Bind(model, nameof(UIModel.IsRunning), new NegateConverter());
			start.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Children.Add(start);
			var stop = MakeButton("Stop", model.RequestStop);
			binding = UI.Bind(model, nameof(UIModel.IsRunning));
			stop.SetBinding(UIElement.IsEnabledProperty, binding);
			result.Children.Add(stop);
			return result;
		}

		static Button MakeButton(string content, Action execute)
		{
			var result = new Button();
			result.Content = content;
			result.Click += (s, e) => execute();
			return result;
		}
	}
}