using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Utility
	{
		public static readonly FontFamily FontFamily = new("Consolas");
		static readonly Dictionary<string, ResourceDictionary> ThemeResources = new();

		internal static string RowEnabledKey = nameof(RowEnabledKey);
		internal static string ForegroundKey = nameof(ForegroundKey);
		internal static string Foreground1Key = nameof(Foreground1Key);
		internal static string Foreground2Key = nameof(Foreground2Key);
		internal static string Foreground3Key = nameof(Foreground3Key);
		internal static string Foreground4Key = nameof(Foreground4Key);
		internal static string RowDisabledKey = nameof(RowDisabledKey);
		internal static string BorderParamKey = nameof(BorderParamKey);
		internal static string ForegroundMixKey = nameof(ForegroundMixKey);
		internal static string BackgroundParamKey = nameof(BackgroundParamKey);

		static Color Multiply(Color color, double factor)
		{
			var r = (byte)Math.Min(color.R * factor, 255);
			var g = (byte)Math.Min(color.G * factor, 255);
			var b = (byte)Math.Min(color.B * factor, 255);
			return Color.FromArgb(255, r, g, b);
		}

		public static ResourceDictionary GetThemeResources(SettingsModel settings, ThemeGroup group)
		{
			string themeColor = GetThemeColor(settings, group);
			if (ThemeResources.TryGetValue(themeColor, out var result)) return result;
			result = new ResourceDictionary();
			var color = (Color)ColorConverter.ConvertFromString(themeColor);
			result.Source = new Uri($"pack://application:,,,/Xt.Synth0.UI;component/Themes/Theme.xaml");
			result.Add(nameof(BackgroundParamKey), MakeParamBackgroundBrush(group));
			result.Add(nameof(RowEnabledKey), new SolidColorBrush(Multiply(color, 1.25)));
			result.Add(nameof(ForegroundKey), new SolidColorBrush(Multiply(color, 1)));
			result.Add(nameof(Foreground1Key), new SolidColorBrush(Multiply(color, 1.25)));
			result.Add(nameof(Foreground2Key), new SolidColorBrush(Multiply(color, 0.75)));
			result.Add(nameof(Foreground3Key), new SolidColorBrush(Multiply(color, 0.5)));
			result.Add(nameof(Foreground4Key), new SolidColorBrush(Multiply(color, 0.25)));
			result.Add(nameof(ForegroundMixKey), new SolidColorBrush(Multiply(color, 0.375)));
			ThemeResources.Add(themeColor, result);
			return result;
		}

		static object MakeParamBackgroundBrush(ThemeGroup group)
		{
			int index = (int)group % 9;
			var result = new ImageBrush();
			result.ImageSource = MakeParamBackgroundSource();
			result.Stretch = Stretch.None;
			result.Opacity = 0.25;
			result.AlignmentX = (AlignmentX)(index / 3);
			result.AlignmentY = (AlignmentY)(index % 3);
			return result;
		}

		static string GetThemeColor(SettingsModel settings, ThemeGroup group)
		=> group switch
		{
			ThemeGroup.Amp => settings.AmpColor,
			ThemeGroup.Lfo => settings.LfoColor,
			ThemeGroup.Plot => settings.PlotColor,
			ThemeGroup.Unit => settings.UnitColor,
			ThemeGroup.Env => settings.EnvelopeColor,
			ThemeGroup.Filter => settings.FilterColor,
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
	}
}