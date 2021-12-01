using System;
using System.Windows;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Utility
	{
		public static ResourceDictionary GetThemeResources(ThemeType theme)
		{
			var location = $"pack://application:,,,/Xt.Synth0.UI;component/Themes/{theme}.xaml";
			var result = new ResourceDictionary();
			result.Source = new Uri(location);
			return result;
		}

		internal static void FocusNext(FocusNavigationDirection direction = FocusNavigationDirection.Next)
		=> (Keyboard.FocusedElement as UIElement)?.MoveFocus(new(direction));
	}
}