using System.Windows;
using System.Windows.Controls;

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

		internal static Grid MakeGrid(int rows, int cols)
		{
			var result = new Grid();
			for (int r = 0; r < rows; r++)
				result.RowDefinitions.Add(MakeRow());
			for (int c = 0; c < cols; c++)
				result.ColumnDefinitions.Add(MakeCol());
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
	}
}