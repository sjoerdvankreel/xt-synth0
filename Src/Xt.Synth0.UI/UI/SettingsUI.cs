using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SettingsUI
	{
		public static void Show(
			SettingsModel settings, AudioModel audio)
		{
			var window = Create.Window(settings);
			window.Content = MakeContent(window, settings, audio);
			window.ShowDialog();
		}

		static UIElement MakeContent(
			Window window, SettingsModel settings, AudioModel audio)
		{
			var result = new StackPanel();
			result.Children.Add(MakeGroup(settings, audio));
			result.Children.Add(MakeOK(window));
			return result;
		}

		static UIElement MakeGroup(
			SettingsModel settings, AudioModel audio)
		{
			var result = new GroupBox();
			result.Header = $"Settings";
			result.Content = MakeGrid(settings, audio);
			return result;
		}

		static UIElement MakeGrid(
			SettingsModel settings, AudioModel audio)
		{
			var result = Create.Grid(4, 2);
			result.Children.Add(Create.Label("Use ASIO", new(0, 0)));
			result.Children.Add(MakeUseAsio(settings, new(0, 1)));
			result.Children.Add(Create.Label("Device", new(1, 0)));
			result.Children.Add(MakeAsioDevice(settings, audio, new(1, 1)));
			result.Children.Add(MakeWasapiDevice(settings, audio, new(1, 1)));
			result.Children.Add(Create.Label("Sample rate", new(2, 0)));
			result.Children.Add(MakeSampleRate(settings, new(2, 1)));
			result.Children.Add(Create.Label("Theme", new(3, 0)));
			result.Children.Add(MakeTheme(settings, new(3, 1)));
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
			SettingsModel settings, AudioModel audio, Cell cell)
		{
			var result = MakeDevice(settings, true,
				nameof(SettingsModel.AsioDeviceId), cell);
			result.ItemsSource = audio.AsioDevices;
			return result;
		}

		static UIElement MakeWasapiDevice(
			SettingsModel settings, AudioModel audio, Cell cell)
		{
			var result = MakeDevice(settings, false,
				nameof(SettingsModel.WasapiDeviceId), cell);
			result.ItemsSource = audio.WasapiDevices;
			return result;
		}

		static UIElement MakeTheme(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = Enum.GetValues<ThemeType>();
			var binding = Bind.To(settings, nameof(settings.Theme));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeUseAsio(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<CheckBox>(cell);
			var binding = Bind.To(settings, nameof(settings.UseAsio));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
			return result;
		}

		static UIElement MakeSampleRate(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = SettingsModel.SampleRates;
			var binding = Bind.To(settings, nameof(settings.SampleRate));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static ComboBox MakeDevice(
			SettingsModel settings, bool asio, string path, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.SelectedValuePath = nameof(DeviceModel.Id);
			var binding = Bind.To(settings, path);
			result.SetBinding(Selector.SelectedValueProperty, binding);
			binding = Bind.To(settings, nameof(SettingsModel.UseAsio), new VisibilityConverter(asio));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}
	}
}