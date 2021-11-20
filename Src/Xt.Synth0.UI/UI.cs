using System.Windows;
using System.Windows.Controls;

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

		internal static Grid MakeGrid(int rows, int columns)
		{
			var result = new Grid();
			for (int r = 0; r < rows; r++)
				result.RowDefinitions.Add(MakeRow());
			for (int c = 0; c < columns; c++)
				result.ColumnDefinitions.Add(MakeColumn());
			return result;
		}

		internal static T MakeElement<T>(int row, int column)
			where T : UIElement, new()
		{
			var result = new T();
			result.SetValue(Grid.RowProperty, row);
			result.SetValue(Grid.ColumnProperty, column);
			return result;
		}

		internal static UIElement MakeLabel(string content, int row, int column)
		{
			var result = MakeElement<Label>(row, column);
			result.Content = content;
			result.VerticalContentAlignment = VerticalAlignment.Top;
			return result;
		}
	}
}