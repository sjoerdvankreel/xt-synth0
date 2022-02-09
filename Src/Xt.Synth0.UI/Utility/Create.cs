using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Create
	{
		internal static Grid Grid(int rows, int cols)
		=> Grid(rows, cols, GridLength.Auto, GridLength.Auto);
		internal static GroupBox ThemedGroup(SettingsModel settings, IUIModel model, object content)
		=> ThemedGroup(settings, model, content, model.Name);

		static RowDefinition Row(GridLength height)
		{
			var result = new RowDefinition();
			result.Height = height;
			return result;
		}

		static ColumnDefinition Col(GridLength width)
		{
			var result = new ColumnDefinition();
			result.Width = width;
			return result;
		}

		internal static GroupBox Group(string header, object content)
		{
			var result = new GroupBox();
			result.Content = content;
			result.Header = header;
			return result;
		}

		internal static Grid Grid(
			int rows, int cols, bool sharedColSize)
		{
			var result = Grid(rows, cols);
			for (int c = 0; c < cols; c++)
				result.ColumnDefinitions[c].SharedSizeGroup = $"Group{c}";
			return result;
		}

		internal static GroupBox ThemedGroup(
			SettingsModel settings, IUIModel model, object content, object header)
		{
			var result = Themed<GroupBox>(settings, model.ThemeGroup);
			result.Content = content;
			result.Header = header;
			return result;
		}

		internal static Label Label(string text)
		{
			var result = new Label();
			result.Content = text;
			return result;
		}

		internal static TextBlock Text(string text)
		{
			var result = new TextBlock();
			result.Text = text;
			return result;
		}

		internal static Label Label(string text, Cell cell)
		{
			var result = Element<Label>(cell);
			result.Content = text;
			return result;
		}

		internal static TextBlock Text(string text, Cell cell)
		{
			var result = Element<TextBlock>(cell);
			result.Text = text;
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
			var binding = Bind.Show(param, min);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		internal static Grid Grid(
			int rows, int cols, GridLength height, GridLength width)
		{
			var result = new Grid();
			for (int r = 0; r < rows; r++)
				result.RowDefinitions.Add(Row(height));
			for (int c = 0; c < cols; c++)
				result.ColumnDefinitions.Add(Col(width));
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

		internal static Border ThemedContent(FrameworkElement content)
		{
			var result = new Border();
			result.Child = content;
			result.SnapsToDevicePixels = true;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Border.BackgroundProperty, Utility.Foreground4Key);
			content.SetResourceReference(Panel.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		public static T Themed<T>(SettingsModel settings, ThemeGroup group)
			where T : FrameworkElement, new()
		{
			var result = new T();
			result.Resources = Utility.GetThemeResources(settings, group);
			settings.PropertyChanged += (s, e) => result.Resources = Utility.GetThemeResources(settings, group);
			return result;
		}

		internal static Window Window(SettingsModel settings, ThemeGroup group, WindowStartupLocation location)
		{
			var result = Themed<Window>(settings, group);
			result.ShowInTaskbar = false;
			result.SnapsToDevicePixels = true;
			result.WindowStyle = WindowStyle.None;
			result.ResizeMode = ResizeMode.NoResize;
			result.WindowStartupLocation = location;
			result.Owner = Application.Current.MainWindow;
			result.SizeToContent = SizeToContent.WidthAndHeight;
			result.SetValue(TextBlock.FontFamilyProperty, Utility.FontFamily);
			return result;
		}
	}
}