using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class Create
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

		internal static GroupBox Group(
			string header, object content)
		{
			var result = new GroupBox();
			result.Content = content;
			result.Header = header;
			return result;
		}

		internal static GroupBox ThemedGroup(
			SettingsModel settings, IThemedModel themed, object content)
		{
			var result = Themed<GroupBox>(settings, themed.Group);
			result.Content = content;
			result.Header = themed.Name;
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

		internal static Grid Grid(
			int rows, int cols, bool sharedColSize)
		{
			var result = Grid(rows, cols);
			for (int c = 0; c < cols; c++)
				result.ColumnDefinitions[c].SharedSizeGroup = $"Group{c}";
			return result;
		}

		internal static Label Label(string text)
		{
			var result = new Label();
			result.Content = text;
			return result;
		}

		internal static Label Label(string text, Cell cell)
		{
			var result = Element<Label>(cell);
			result.Content = text;
			return result;
		}

		internal static TextBlock Text(string text)
		{
			var result = new TextBlock();
			result.Text = text;
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

		internal static Window Window(SettingsModel settings, ThemeGroup group)
		{
			var result = Themed<Window>(settings, group);
			result.ShowInTaskbar = false;
			result.SnapsToDevicePixels = true;
			result.WindowStyle = WindowStyle.None;
			result.ResizeMode = ResizeMode.NoResize;
			result.Owner = Application.Current.MainWindow;
			result.SizeToContent = SizeToContent.WidthAndHeight;
			result.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			result.SetValue(TextBlock.FontFamilyProperty, Utility.FontFamily);
			return result;
		}

		internal static Border ThemedContent(FrameworkElement content)
		{
			var result = new Border();
			result.Child = content;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Border.BackgroundProperty, Utility.Foreground4Key);
			content.SetResourceReference(Panel.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		static void OnSettingsPropertyChanged(SettingsModel settings,
			ThemeGroup group, FrameworkElement element, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(SettingsModel.ThemeType) ||
				e.PropertyName == nameof(SettingsModel.ThemeColor))
				element.Resources = Utility.GetThemeResources(settings, group);
		}

		public static T Themed<T>(SettingsModel settings, ThemeGroup group)
			where T : FrameworkElement, new()
		{
			var result = new T();
			result.Resources = Utility.GetThemeResources(settings, group);
			settings.PropertyChanged += (s, e) => OnSettingsPropertyChanged(settings, group, result, e);
			return result;
		}
	}
}