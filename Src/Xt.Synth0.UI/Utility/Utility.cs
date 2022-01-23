using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Utility
	{
		static readonly Dictionary<ThemeType, ResourceDictionary> ThemeResources = new();

		public static readonly FontFamily FontFamily = new("Consolas");
		internal static string RowEnabledKey = nameof(RowEnabledKey);
		internal static string ForegroundKey = nameof(ForegroundKey);
		internal static string Foreground1Key = nameof(Foreground1Key);
		internal static string Foreground2Key = nameof(Foreground2Key);
		internal static string Foreground3Key = nameof(Foreground3Key);
		internal static string Foreground4Key = nameof(Foreground4Key);
		internal static string RowDisabledKey = nameof(RowDisabledKey);
		internal static string BorderParamKey = nameof(BorderParamKey);
		internal static string BackgroundParamKey = nameof(BackgroundParamKey);

		static ThemeType GroupType(ThemeGroup group) => group switch
		{
			ThemeGroup.Global => ThemeType.Rose,
			ThemeGroup.Lfos => ThemeType.Yellow,
			ThemeGroup.Envs => ThemeType.Spring,
			ThemeGroup.Units => ThemeType.Orange,
			ThemeGroup.Settings => ThemeType.White,
			ThemeGroup.Plot => ThemeType.Chartreuse,
			ThemeGroup.EditPattern => ThemeType.Cyan,
			ThemeGroup.MonitorControl => ThemeType.Azure,
			_ => throw new InvalidOperationException()
		};

		internal static void FocusDown()
		{
			var request = new TraversalRequest(FocusNavigationDirection.Down);
			(Keyboard.FocusedElement as UIElement)?.MoveFocus(request);
		}

		public static ResourceDictionary GetThemeResources(ThemeType theme, ThemeGroup group)
		{
			var effectiveTheme = theme != ThemeType.Grouped ? theme : GroupType(group);
			if (ThemeResources.TryGetValue(effectiveTheme, out var result)) return result;
			var location = $"pack://application:,,,/Xt.Synth0.UI;component/Themes/{effectiveTheme}.xaml";
			result = new ResourceDictionary();
			result.Source = new Uri(location);
			ThemeResources.Add(effectiveTheme, result);
			return result;
		}
	}
}