using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Xt.Synth0.UI
{
	static class UI
	{
		internal const int Margin = 5;

		static RowDefinition MakeRow()
		{
			var result = new RowDefinition();
			result.Height = GridLength.Auto;
			return result;
		}

		static ColumnDefinition MakeColumn()
		{
			var result = new ColumnDefinition();
			result.Width = GridLength.Auto;
			return result;
		}

		internal static MenuItem MakeItem(string header)
		{
			var result = new MenuItem();
			result.Header = header;
			return result;
		}

		internal static Grid MakeGrid(int rows, int columns)
		{
			var result = new Grid();
			for (int r = 0; r < rows; r++)
				result.RowDefinitions.Add(MakeRow());
			for (int c = 0; c < columns; c++)
				result.ColumnDefinitions.Add(MakeColumn());
			return result;
		}

		internal static T MakeElement<T>(
			int row, int column, int rowSpan = 1, int columnSpan = 1)
			where T : UIElement, new()
		{
			var result = new T();
			result.SetValue(Grid.RowProperty, row);
			result.SetValue(Grid.ColumnProperty, column);
			result.SetValue(Grid.RowSpanProperty, rowSpan);
			result.SetValue(Grid.ColumnSpanProperty, columnSpan);
			return result;
		}

		internal static UIElement MakeLabel(string content, int row, int column)
		{
			var result = MakeElement<Label>(row, column);
			result.Content = content;
			result.VerticalContentAlignment = VerticalAlignment.Top;
			return result;
		}

		internal static MenuItem MakeItem(ICommand command, string header, Action execute)
		{
			var result = MakeItem(header);
			result.Command = command;
			var binding = new CommandBinding();
			binding.Command = command;
			binding.Executed += (s, e) => execute();
			result.CommandBindings.Add(binding);
			return result;
		}
	}
}