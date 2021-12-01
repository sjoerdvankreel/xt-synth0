using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class OptionsUI
	{
		public static void Show(
			OptionsModel options, AudioModel audio)
		{
			var window = Create.Window(options);
			window.Content = MakeContent(window, options, audio);
			window.ShowDialog();
		}

		static UIElement MakeContent(
			Window window, OptionsModel options, AudioModel audio)
		{
			var result = new StackPanel();
			result.Children.Add(MakeGroup(window, options, audio));
			result.Children.Add(MakeOK(window));
			return result;
		}

		static UIElement MakeGroup(
			Window window, OptionsModel options, AudioModel audio)
		{
			var result = new GroupBox();
			result.Header = $"Options";
			result.Content = MakeGrid(window, options, audio);
			return result;
		}

		static UIElement MakeGrid(
			Window window, OptionsModel options, AudioModel audio)
		{
			var result = Create.Grid(4, 2);
			result.Children.Add(Create.Label("Theme", new(0, 0)));
			result.Children.Add(MakeTheme(options, new(0, 1)));
			result.Children.Add(Create.Label("Use ASIO", new(1, 0)));
			result.Children.Add(MakeUseAasio(options, new(1, 1)));
			result.Children.Add(Create.Label("Device", new(2, 0)));
			result.Children.Add(MakeAsioDevice(options, audio, new(2, 1)));
			result.Children.Add(MakeWasapiDevice(options, audio, new(2, 1)));
			result.Children.Add(Create.Label("Sample rate", new(3, 0)));
			result.Children.Add(MakeSampleRate(options, new(3, 1)));
			return result;
		}

		static UIElement MakeOK(Window window)
		{
			var result = new Button();
			result.Content = "OK";
			result.Click += (s, e) => window.Close();
			result.HorizontalAlignment = HorizontalAlignment.Right;
			return result;
		}

		static UIElement MakeAsioDevice(
			OptionsModel options, AudioModel audio, Cell cell)
		{
			var result = MakeDevice(options, true,
				nameof(OptionsModel.AsioDeviceId), cell);
			result.ItemsSource = audio.AsioDevices;
			return result;
		}

		static UIElement MakeWasapiDevice(
			OptionsModel options, AudioModel audio, Cell cell)
		{
			var result = MakeDevice(options, false,
				nameof(OptionsModel.WasapiDeviceId), cell);
			result.ItemsSource = audio.WasapiDevices;
			return result;
		}

		static UIElement MakeTheme(OptionsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = Enum.GetValues<ThemeType>();
			var binding = Bind.To(model, nameof(model.Theme));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeUseAasio(OptionsModel model, Cell cell)
		{
			var result = Create.Element<CheckBox>(cell);
			var binding = Bind.To(model, nameof(model.UseAsio));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
			return result;
		}

		static UIElement MakeSampleRate(OptionsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = OptionsModel.SampleRates;
			var binding = Bind.To(model, nameof(model.SampleRate));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static ComboBox MakeDevice(
			OptionsModel options, bool asio, string path, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.SelectedValuePath = nameof(DeviceModel.Id);
			result.DisplayMemberPath = nameof(DeviceModel.Name);
			var binding = Bind.To(options, path);
			result.SetBinding(Selector.SelectedValueProperty, binding);
			binding = Bind.To(options, nameof(OptionsModel.UseAsio), new VisibilityConverter(asio));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}
	}
}