using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Utility
	{
		public static readonly FontFamily FontFamily = new("Consolas");

		internal static void FocusDown()
		{
			var request = new TraversalRequest(FocusNavigationDirection.Down);
			(Keyboard.FocusedElement as UIElement)?.MoveFocus(request);
		}

		public static ResourceDictionary GetThemeResources(ThemeType theme)
		{
			var location = $"pack://application:,,,/Xt.Synth0.UI;component/Themes/{theme}.xaml";
			var result = new ResourceDictionary();
			result.Source = new Uri(location);
			return result;
		}
	}
}