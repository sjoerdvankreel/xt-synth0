using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SettingsUI
	{
		public static void Show(SettingsModel model)
		{
			var window = Create.Window(model);
			window.Content = MakeContent(window, model);
			window.ShowDialog();
		}

		static UIElement MakeContent(Window window, SettingsModel model)
		{
			var result = new StackPanel();
			result.Children.Add(MakeGroup(model));
			result.Children.Add(MakeOK(window));
			return result;
		}

		static UIElement MakeGroup(SettingsModel model)
		{
			var result = new GroupBox();
			result.Header = $"Settings";
			result.Content = MakeGrid(model);
			return result;
		}

		static UIElement MakeGrid(SettingsModel model)
		{
			var result = Create.Grid(5, 2);
			result.Children.Add(Create.Label("Use ASIO", new(0, 0)));
			result.Children.Add(MakeUseAsio(model, new(0, 1)));
			result.Children.Add(Create.Label("Device", new(1, 0)));
			result.Children.Add(MakeAsioDevice(model, new(1, 1)));
			result.Children.Add(MakeWasapiDevice(model, new(1, 1)));
			result.Children.Add(Create.Label("Sample rate", new(2, 0)));
			result.Children.Add(MakeSampleRate(model, new(2, 1)));
			result.Children.Add(Create.Label("Buffer size (ms)", new(3, 0)));
			result.Children.Add(MakeBufferSize(model, new(3, 1)));
			result.Children.Add(Create.Label("Theme", new(4, 0)));
			result.Children.Add(MakeTheme(model, new(4, 1)));
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

		static UIElement MakeAsioDevice(SettingsModel model, Cell cell)
		{
			var result = MakeDevice(model, true,
				nameof(SettingsModel.AsioDeviceId), cell);
			result.ItemsSource = AudioModel.AsioDevices;
			return result;
		}

		static UIElement MakeWasapiDevice(SettingsModel model, Cell cell)
		{
			var result = MakeDevice(model, false,
				nameof(SettingsModel.WasapiDeviceId), cell);
			result.ItemsSource = AudioModel.WasapiDevices;
			return result;
		}

		static UIElement MakeTheme(SettingsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = Enum.GetValues<ThemeType>();
			var binding = Bind.To(model, nameof(model.Theme));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeUseAsio(SettingsModel model, Cell cell)
		{
			var result = Create.Element<CheckBox>(cell);
			var binding = Bind.To(model, nameof(model.UseAsio));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
			return result;
		}

		static UIElement MakeBufferSize(SettingsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = AudioModel.BufferSizes;
			result.SelectedValuePath = nameof(BufferModel.Size);
			var binding = Bind.To(model, nameof(model.BufferSize));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeSampleRate(SettingsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = AudioModel.SampleRates;
			result.SelectedValuePath = nameof(RateModel.Rate);
			var binding = Bind.To(model, nameof(model.SampleRate));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static ComboBox MakeDevice(
			SettingsModel model, bool asio, string path, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.SelectedValuePath = nameof(DeviceModel.Id);
			var binding = Bind.To(model, path);
			result.SetBinding(Selector.SelectedValueProperty, binding);
			binding = Bind.To(model, nameof(SettingsModel.UseAsio), new VisibilityConverter(asio));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}
	}
}