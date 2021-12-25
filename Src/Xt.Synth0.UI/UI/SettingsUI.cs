﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SettingsUI
	{
		public static event EventHandler ShowASIOControlPanel;
		public static event EventHandler<QueryFormatSupportEventArgs> QueryFormatSupport;

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
			var result = Create.Grid(7, 2);
			result.Children.Add(Create.Label("Theme", new(0, 0)));
			result.Children.Add(MakeTheme(model, new(0, 1)));
			result.Children.Add(Create.Label("Use ASIO", new(1, 0)));
			result.Children.Add(MakeAsio(model, new(1, 1)));
			result.Children.Add(Create.Label("Device", new(2, 0)));
			result.Children.Add(MakeAsioDevice(model, new(2, 1)));
			result.Children.Add(MakeWasapiDevice(model, new(2, 1)));
			result.Children.Add(Create.Label("Bit depth", new(3, 0)));
			result.Children.Add(MakeBitDepth(model, new(3, 1)));
			result.Children.Add(Create.Label("Sample rate", new(4, 0)));
			result.Children.Add(MakeSampleRate(model, new(4, 1)));
			result.Children.Add(Create.Label("Buffer size (ms)", new(5, 0)));
			result.Children.Add(MakeBufferSize(model, new(5, 1)));
			result.Children.Add(Create.Label("Format support", new(6, 0)));
			result.Children.Add(MakeFormatSupport(model, new(6, 1)));
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

		static UIElement MakeTheme(SettingsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = Enum.GetValues<ThemeType>();
			var binding = Bind.To(model, nameof(model.Theme));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeAsio(SettingsModel model, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Children.Add(MakeUseAsio(model));
			result.Children.Add(MakeAsioControlPanel(model));
			return result;
		}

		static UIElement MakeUseAsio(SettingsModel model)
		{
			var result = new CheckBox();
			var binding = Bind.To(model, nameof(model.UseAsio));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
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

		static UIElement MakeBufferSize(SettingsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = AudioModel.BufferSizes;
			result.SelectedValuePath = nameof(BufferModel.Size);
			var binding = Bind.To(model, nameof(model.BufferSize));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeBitDepth(SettingsModel model, Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.ItemsSource = AudioModel.BitDepths;
			result.SelectedValuePath = nameof(DepthModel.Depth);
			var binding = Bind.To(model, nameof(model.BitDepth));
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

		static UIElement MakeAsioControlPanel(SettingsModel model)
		{
			var result = new Button();
			result.Content = "Control panel";
			result.Click += (s, e) => ShowASIOControlPanel?.Invoke(null, EventArgs.Empty);
			var binding = Bind.To(model, nameof(SettingsModel.UseAsio), new VisibilityConverter(true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeFormatSupport(SettingsModel model, Cell cell)
		{
			var result = Create.Element<Label>(cell);
			result.Content = DoQueryFormatSupport();
			model.PropertyChanged += (s, e) => UpdateDeviceBuffer(result, e);
			return result;
		}

		static string DoQueryFormatSupport()
		{
			var args = new QueryFormatSupportEventArgs();
			QueryFormatSupport?.Invoke(null, args);
			if (!args.IsSupported) return "Not supported";
			string min = args.MinBuffer.ToString("N2");
			string max = args.MaxBuffer.ToString("N2");
			string @default = args.DefaultBuffer.ToString("N2");
			return $"Supported, buffer size: {min} .. {max}ms";
		}

		static void UpdateDeviceBuffer(Label label, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(SettingsModel.UseAsio) ||
				e.PropertyName == nameof(SettingsModel.BitDepth) ||
				e.PropertyName == nameof(SettingsModel.SampleRate) ||
				e.PropertyName == nameof(SettingsModel.AsioDeviceId) ||
				e.PropertyName == nameof(SettingsModel.WasapiDeviceId))
				label.Content = DoQueryFormatSupport();
		}
	}
}