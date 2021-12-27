using System.Windows;
using System.Windows.Controls;

namespace Xt.Synth0.UI
{
	public static class UIExtensions
	{
		public static void Add(this Panel self, UIElement child)
		=> self.Children.Add(child);

		internal static void Add(this Grid self, UIElement child, Cell cell)
		{
			self.Children.Add(child);
			child.SetValue(Grid.RowProperty, cell.Row);
			child.SetValue(Grid.ColumnProperty, cell.Col);
			child.SetValue(Grid.RowSpanProperty, cell.RowSpan);
			child.SetValue(Grid.ColumnSpanProperty, cell.ColSpan);
		}

		public static void Add(this DockPanel self, UIElement child, Dock dock)
		{
			self.Children.Add(child);
			child.SetValue(DockPanel.DockProperty, dock);
		}
	}
}