using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class Create
	{
		static RowDefinition Row()
		{
			var result = new RowDefinition();
			result.Height = GridLength.Auto;
			return result;
		}

		static ColumnDefinition Col()
		{
			var result = new ColumnDefinition();
			result.Width = GridLength.Auto;
			return result;
		}

		internal static Grid Grid(int rows, int cols)
		{
			var result = new Grid();
			for (int r = 0; r < rows; r++)
				result.RowDefinitions.Add(Row());
			for (int c = 0; c < cols; c++)
				result.ColumnDefinitions.Add(Col());
			return result;
		}

		internal static Label Label(string text, Cell cell)
		{
			var result = Element<Label>(cell);
			result.Content = text;
			return result;
		}

		internal static T PatternCell<T>(Cell cell)
			where T : FrameworkElement, new()
		{
			var result = Element<T>(cell);
			result.Focusable = true;
			result.Margin = new Thickness(0, 0, 0, 2);
			result.MouseLeftButtonDown += (s, e) => result.Focus();
			return result;
		}

		internal static UIElement Divider(
			Cell cell, Param param, int min)
		{
			var result = Element<TextBlock>(cell);
			result.Text = " ";
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(param, min));
			return result;
		}

		internal static T Element<T>(Cell cell)
			where T : UIElement, new()
		{
			var result = new T();
			result.SetValue(System.Windows.Controls.Grid.RowProperty, cell.Row);
			result.SetValue(System.Windows.Controls.Grid.ColumnProperty, cell.Col);
			result.SetValue(System.Windows.Controls.Grid.RowSpanProperty, cell.RowSpan);
			result.SetValue(System.Windows.Controls.Grid.ColumnSpanProperty, cell.ColSpan);
			return result;
		}

		internal static Window Window(OptionsModel model)
		{
			var result = new Window();
			result.ShowInTaskbar = false;
			result.SnapsToDevicePixels = true;
			result.WindowStyle = WindowStyle.None;
			result.ResizeMode = ResizeMode.NoResize;
			result.Owner = Application.Current.MainWindow;
			result.SizeToContent = SizeToContent.WidthAndHeight;
			result.Resources = Utility.GetThemeResources(model.Theme);
			result.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			result.SetValue(TextBlock.FontFamilyProperty, Utility.FontFamily);
			model.ThemeChanged += (s, e) => result.Resources = Utility.GetThemeResources(model.Theme);
			return result;
		}
	}
}