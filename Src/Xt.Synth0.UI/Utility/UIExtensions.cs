using System.Windows;
using System.Windows.Controls;

namespace Xt.Synth0.UI
{
	public static class UIExtensions
	{
		public static void Add(this Panel self, UIElement child)
		=> self.Children.Add(child);

		public static void Add(this DockPanel self, UIElement child, Dock dock)
		{
			self.Children.Add(child);
			child.SetValue(DockPanel.DockProperty, dock);
		}
	}
}