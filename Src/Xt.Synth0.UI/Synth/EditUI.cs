using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class EditUI
	{
		internal static void Show(UIModel model, Param param)
		{
			var window = new Window();
			window.ResizeMode = ResizeMode.NoResize;
			window.Content = MakeContent(window, param);
			window.Owner = Application.Current.MainWindow;
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.Resources = UI.GetThemeResources(model.Theme);
			window.Title = $"{param.Info.Min} .. {param.Info.Max}";
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowDialog();
		}

		static void AddDocked(DockPanel panel, UIElement element)
		{
			element.SetValue(DockPanel.DockProperty, Dock.Right);
			panel.Children.Add(element);
		}

		static UIElement MakeContent(Window window, Param param)
		{
			var result = new DockPanel();
			var box = MakeTextBox(param);
			AddDocked(result, MakeCancel(window, param));
			AddDocked(result, MakeOK(window, box, param));
			AddDocked(result, box);
			return result;
		}

		static TextBox MakeTextBox(Param param)
		{
			var result = new TextBox();
			result.Text = param.Value.ToString();
			return result;
		}

		static UIElement MakeCancel(Window window, Param param)
		{
			var result = new Button();
			result.Content = "Cancel";
			result.Click += (s, e) => window.Close();
			return result;
		}

		static UIElement MakeOK(Window window, TextBox box, Param param)
		{
			var result = new Button();
			result.Content = "OK";
			result.Click += (s, e) => Edit(window, param, box.Text);
			result.SetValue(DockPanel.DockProperty, Dock.Right);
			return result;
		}

		static void Edit(Window window, Param param, string value)
		{
			if (!int.TryParse(value, out int newValue)) return;
			if (newValue < param.Info.Min || newValue > param.Info.Max) return;
			param.Value = newValue;
			window.Close();
		}
	}
}