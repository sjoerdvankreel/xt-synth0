using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class UI
	{
		public static readonly FontFamily FontFamily = new FontFamily("Consolas");

		static RowDefinition MakeRow()
		{
			var result = new RowDefinition();
			result.Height = GridLength.Auto;
			return result;
		}

		static ColumnDefinition MakeCol()
		{
			var result = new ColumnDefinition();
			result.Width = GridLength.Auto;
			return result;
		}

	

		internal static UIElement MakeText(string text)
		{
			var result = new TextBlock();
			result.Text = text;
			return result;
		}

		internal static UIElement MakeText(Binding binding)
		{
			var result = new TextBlock();
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}

		internal static Grid MakeGrid(int rows, int cols)
		{
			var result = new Grid();
			for (int r = 0; r < rows; r++)
				result.RowDefinitions.Add(MakeRow());
			for (int c = 0; c < cols; c++)
				result.ColumnDefinitions.Add(MakeCol());
			return result;
		}

		internal static T MakePatternCell<T>(Cell cell)
			where T : FrameworkElement, new()
		{
			var result = MakeElement<T>(cell);
			result.Focusable = true;
			result.Margin = new Thickness(0, 0, 0, 2);
			result.MouseLeftButtonDown += (s, e) => result.Focus();
			return result;
		}

		internal static T MakeElement<T>(Cell cell)
			where T : UIElement, new()
		{
			var result = new T();
			result.SetValue(Grid.RowProperty, cell.Row);
			result.SetValue(Grid.ColumnProperty, cell.Col);
			result.SetValue(Grid.RowSpanProperty, cell.RowSpan);
			result.SetValue(Grid.ColumnSpanProperty, cell.ColSpan);
			return result;
		}

		internal static UIElement MakeDivider(
			Cell cell, Param param, int min)
		{
			var result = MakeElement<TextBlock>(cell);
			result.Text = " ";
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(param, min));
			return result;
		}

		internal static void FocusNext(FocusNavigationDirection direction)
		{
			if (Keyboard.FocusedElement is UIElement e)
				e.MoveFocus(new(direction));
		}

		public static ResourceDictionary GetThemeResources(ThemeType type)
		{
			ResourceDictionary result = new();
			var uri = $"pack://application:,,,/Xt.Synth0.UI;component/Themes/{type}.xaml";
			result.Source = new Uri(uri);
			return result;
		}
	}
}