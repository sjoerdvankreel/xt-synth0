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
			ThemeGroup.Lfos => ThemeType.Slate,
			ThemeGroup.Units => ThemeType.Azure,
			ThemeGroup.Envs => ThemeType.Yellow,
			ThemeGroup.Global => ThemeType.Spring,
			ThemeGroup.Settings => ThemeType.White,
			ThemeGroup.Plot => ThemeType.Chartreuse,
			ThemeGroup.EditPattern => ThemeType.Orange,
			ThemeGroup.MonitorControl => ThemeType.Cyan,
			_ => throw new InvalidOperationException()
		};

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
			var result =  Color.FromArgb(255, r, g, b);
			return result;
		}

		public static ResourceDictionary GetThemeResources(SettingsModel settings, ThemeGroup group)
		{
			var result = new ResourceDictionary();
			var color = (Color)ColorConverter.ConvertFromString(settings.ThemeColor);
			result.Source = new Uri($"pack://application:,,,/Xt.Synth0.UI;component/Themes/Theme.xaml");
			result.Add(nameof(RowEnabledKey), new SolidColorBrush(Multiply(color, 1.25)));
			result.Add(nameof(ForegroundKey), new SolidColorBrush(Multiply(color, 1)));
			result.Add(nameof(Foreground1Key), new SolidColorBrush(Multiply(color, 1.25)));
			result.Add(nameof(Foreground2Key), new SolidColorBrush(Multiply(color, 0.75)));
			result.Add(nameof(Foreground3Key), new SolidColorBrush(Multiply(color, 0.5)));
			result.Add(nameof(Foreground4Key), new SolidColorBrush(Multiply(color, 0.25)));

			var bmi = new BitmapImage();
			bmi.BeginInit();
			bmi.UriSource = new Uri("pack://application:,,,/Xt.Synth0.UI;component/Themes/Noise.png");
			bmi.EndInit();

			result.Add(nameof(BackgroundParamKey), new ImageBrush
			{
				ImageSource = bmi,
				Stretch = Stretch.None,
				Opacity = 0.25,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top
			});
			return result;
			/*
			 * 
			 * 		 <Brush x:Key="RowEnabledKey">#00A0FF</Brush>
    <Brush x:Key="ForegroundKey">#0080FF</Brush>
    <Brush x:Key="Foreground1Key">#00A0FF</Brush>
    <Brush x:Key="Foreground2Key">#0060FF</Brush>
    <Brush x:Key="Foreground3Key">#004080</Brush>
    <Brush x:Key="Foreground4Key">#001020</Brush>
    <ImageBrush x:Key="BackgroundParamKey" ImageSource="Noise.png" Stretch="None" Opacity="0.25" AlignmentX="Left" AlignmentY="Top"/>


			var effectiveTheme = theme != ThemeType.Grouped ? theme : GroupType(group);
			if (ThemeResources.TryGetValue(effectiveTheme, out var result)) return result;
			var location = $"pack://application:,,,/Xt.Synth0.UI;component/Themes/{effectiveTheme}.xaml";
			result = new ResourceDictionary();
			result.Source = new Uri(location);
			ThemeResources.Add(effectiveTheme, result);
			return result;
			*/
		}
	}
}