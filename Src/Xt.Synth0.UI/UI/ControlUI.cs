﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace Xt.Synth0.UI
{
	public static class ControlUI
	{
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
			result.Children.Add(MakeButton("Start", null));
			var stop = MakeButton("Stop", null);
			stop.IsEnabled = false;
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