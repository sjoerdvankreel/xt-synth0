using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class UI
	{
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

		internal static Binding Bind(Param param)
		{
			var result = new Binding(nameof(Param.Value));
			result.Source = param;
			return result;
		}

		internal static Binding Format(Param param)
		{
			var result = Bind(param);
			result.Converter = new ParamFormatter(param.Info);
			return result;
		}

		internal static Binding Show(Param param, int min)
		{
			var result = Bind(param);
			result.Converter = new ShowConverter(min);
			return result;
		}

		internal static BindingBase Format(Param first, 
			Param second, MultiConverter<int, int, string> formatter)
		{
			var result = new MultiBinding();
			result.Converter = formatter;
			result.Bindings.Add(Bind(first));
			result.Bindings.Add(Bind(second));
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
			result.SetBinding(UIElement.VisibilityProperty, Show(param, min));
			return result;
		}

		internal static void FocusNext(FocusNavigationDirection direction)
		{
			if (Keyboard.FocusedElement is UIElement e)
				e.MoveFocus(new(direction));
		}
	}
}