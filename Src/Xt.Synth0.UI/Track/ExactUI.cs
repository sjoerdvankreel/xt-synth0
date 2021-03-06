using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class ExactUI
	{
		internal static void Show(SettingsModel settings, ThemeGroup group, Param param)
		{
			var window = Create.Window(settings, group, WindowStartupLocation.CenterOwner);
			window.Content = MakeGroup(window, param);
			window.ShowDialog();
		}

		static UIElement MakeGroup(Window window, Param param)
		=> Create.Group(param.Info.Range(false), MakeContent(window, param));

		static UIElement MakeContent(Window window, Param param)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			var box = MakeTextBox(param);
			result.Add(box);
			result.Add(MakeOK(window, box, param));
			result.Add(MakeCancel(window));
			return result;
		}

		static TextBox MakeTextBox(Param param)
		{
			var result = new TextBox();
			result.Width = 50.0;
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