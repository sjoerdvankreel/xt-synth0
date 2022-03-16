using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Xt.Synth0.UI
{
    public static class UIExtensions
    {
        public static T Add<T>(this Panel self, T child)
            where T : UIElement
        {
            self.Children.Add(child);
            return child;
        }

        internal static T Add<T>(this Grid self, T child, Cell cell)
            where T : UIElement
        {
            self.Children.Add(child);
            child.SetValue(Grid.RowProperty, cell.Row);
            child.SetValue(Grid.ColumnProperty, cell.Col);
            child.SetValue(Grid.RowSpanProperty, cell.RowSpan);
            child.SetValue(Grid.ColumnSpanProperty, cell.ColSpan);
            return child;
        }

        public static T Add<T>(this DockPanel self, T child, Dock dock)
            where T : UIElement
        {
            self.Children.Add(child);
            child.SetValue(DockPanel.DockProperty, dock);
            return child;
        }

        internal static void AddRange<T>(this Panel self, IEnumerable<T> children)
            where T : UIElement
        {
            foreach (var child in children)
                self.Add(child);
        }
    }
}