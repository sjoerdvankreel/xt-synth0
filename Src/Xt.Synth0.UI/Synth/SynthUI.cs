﻿using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SynthUI
	{
		public static UIElement Make(SynthModel model)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(MakeLeft(model));
			result.Children.Add(PatternUI.Make(model));
			return result;
		}

		static void AddDocked(Panel panel, UIElement element)
		{
			panel.Children.Add(element);
			element.SetValue(DockPanel.DockProperty, Dock.Top);
		}

		static UIElement MakeLeft(SynthModel model)
		{
			var result = new DockPanel();
			AddDocked(result, GroupUI.Make(model, model.Unit1));
			AddDocked(result, GroupUI.Make(model, model.Unit2));
			AddDocked(result, GroupUI.Make(model, model.Unit3));
			AddDocked(result, GroupUI.Make(model, model.Editor));
			AddDocked(result, GroupUI.Make(model, model.Global));
			return result;
		}
	}
}