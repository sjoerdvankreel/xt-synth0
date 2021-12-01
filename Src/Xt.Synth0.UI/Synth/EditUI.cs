using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class EditUI
	{
		internal static void Show(OptionsModel model, Param param)
		{
			var window = new Window();
			window.ResizeMode = ResizeMode.NoResize;
			window.Content = MakeGroup(window, param);
			window.Title = $"Edit {param.Info.Detail}";
			window.Owner = Application.Current.MainWindow;
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.Resources = Utility.GetThemeResources(model.Theme);
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowDialog();
		}

		static UIElement MakeGroup(Window window, Param param)
		{
			var result = new GroupBox();
			result.Content = MakeContent(window, param);
			result.Header = $"{param.Info.Min} to {param.Info.Max}";
			return result;
		}

		static UIElement MakeContent(Window window, Param param)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			var box = MakeTextBox(param);
			result.Children.Add(box);
			result.Children.Add(MakeOK(window, box, param));
			result.Children.Add(MakeCancel(window));
			return result;
		}

		static TextBox MakeTextBox(Param param)
		{
			var result = new TextBox();
			result.Text = param.Value.ToString();
			return result;
		}

		static UIElement MakeCancel(Window window)
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