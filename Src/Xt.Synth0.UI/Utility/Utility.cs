using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Utility
	{
		static ResourceDictionary _genericResources;
		static readonly Dictionary<string, ResourceDictionary> ThemeResources = new();

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

		internal static void FocusDown()
		{
			var request = new TraversalRequest(FocusNavigationDirection.Down);
			(Keyboard.FocusedElement as UIElement)?.MoveFocus(request);
		}

		static Color Multiply(Color color, double factor)
		{
			var r = (byte)Math.Min(color.R * factor, 255);
			var g = (byte)Math.Min(color.G * factor, 255);
			var b = (byte)Math.Min(color.B * factor, 255);
			return Color.FromArgb(255, r, g, b);
		}

		public static ResourceDictionary GetThemeResources(SettingsModel settings, ThemeGroup group)
		{
			if (settings.ThemeType == ThemeType.Generic) return GetGenericResources();
			string themeColor = GetThemeColor(settings, group);
			if (ThemeResources.TryGetValue(themeColor, out var result)) return result;
			result = new ResourceDictionary();
			var color = (Color)ColorConverter.ConvertFromString(settings.ThemeColor);
			result.Source = new Uri($"pack://application:,,,/Xt.Synth0.UI;component/Themes/Theme.xaml");
			result.Add(nameof(BackgroundParamKey), MakeParamBackgroundBrush());
			result.Add(nameof(RowEnabledKey), new SolidColorBrush(Multiply(color, 1.25)));
			result.Add(nameof(ForegroundKey), new SolidColorBrush(Multiply(color, 1)));
			result.Add(nameof(Foreground1Key), new SolidColorBrush(Multiply(color, 1.25)));
			result.Add(nameof(Foreground2Key), new SolidColorBrush(Multiply(color, 0.75)));
			result.Add(nameof(Foreground3Key), new SolidColorBrush(Multiply(color, 0.5)));
			result.Add(nameof(Foreground4Key), new SolidColorBrush(Multiply(color, 0.25)));
			ThemeResources.Add(themeColor, result);
			return result;
		}

		static object MakeParamBackgroundBrush()
		{
			var result = new ImageBrush();
			result.ImageSource = MakeParamBackgroundSource();
			result.Stretch = Stretch.None;
			result.Opacity = 0.25;
			result.AlignmentX = AlignmentX.Left;
			result.AlignmentY = AlignmentY.Top;
			return result;
		}

		static string GetThemeColor(SettingsModel settings, ThemeGroup group)
		=> settings.ThemeType switch
		{
			ThemeType.Themed => settings.ThemeColor,
			ThemeType.Grouped => GetGroupColor(settings, group),
			_ => throw new InvalidOperationException()
		};


		static string GetGroupColor(SettingsModel settings, ThemeGroup group)
		=> group switch
		{
			ThemeGroup.Lfos => settings.LfoColor,
			ThemeGroup.Envs => settings.EnvelopeColor,
			ThemeGroup.Plot => settings.PlotColor,
			ThemeGroup.Units => settings.UnitColor,
			ThemeGroup.Global => settings.GlobalColor,
			ThemeGroup.Control => settings.ControlColor,
			ThemeGroup.Pattern => settings.PatternColor,
			ThemeGroup.Settings => settings.SettingsColor,
			_ => throw new InvalidOperationException()
		};

		static ImageSource MakeParamBackgroundSource()
		{
			var result = new BitmapImage();
			result.BeginInit();
			result.UriSource = new Uri("pack://application:,,,/Xt.Synth0.UI;component/Themes/Noise.png");
			result.EndInit();
			return result;
		}

		static ResourceDictionary GetGenericResources()
		{
			if (_genericResources != null) return _genericResources;
			_genericResources = new ResourceDictionary();
			_genericResources.Source = new Uri($"pack://application:,,,/Xt.Synth0.UI;component/Themes/Generic.xaml");
			return _genericResources;
		}
	}
}