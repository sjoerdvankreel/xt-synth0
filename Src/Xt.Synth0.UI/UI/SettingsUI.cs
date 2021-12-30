using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class SettingsUI
	{
		const int ComboWidth = 200;
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
			result.Add(MakeGroup(model));
			result.Add(MakeOK(window));
			return result;
		}

		static UIElement MakeGroup(SettingsModel model)
		=> Create.Group("Settings", MakeContent(model));

		static UIElement MakeContent(SettingsModel model)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Vertical;
			result.Add(MakeUpper(model));
			result.Add(MakeCenter(model));
			result.Add(MakeLower(model));
			result.SetValue(Grid.IsSharedSizeScopeProperty, true);
			return result;
		}

		static UIElement MakeUpper(SettingsModel model)
		{
			var result = Create.Grid(4, 2, true);
			result.Add(Create.Label("Theme", new(0, 0)));
			result.Add(MakeTheme(model, new(0, 1)));
			result.Add(Create.Label("Bit depth", new(1, 0)));
			result.Add(MakeBitDepth(model, new(1, 1)));
			result.Add(Create.Label("Sample rate", new(2, 0)));
			result.Add(MakeSampleRate(model, new(2, 1)));
			result.Add(Create.Label("Write to disk", new(3, 0)));
			result.Add(MakeWriteToDisk(model, new(3, 1)));
			return result;
		}

		static UIElement MakeCenter(SettingsModel model)
		{
			var result = Create.Grid(1, 2, true);
			result.Add(Create.Label("Output path", new(0, 0)));
			result.Add(MakeOutputPath(model, new(0, 1)));
			var binding = Bind.To(model, nameof(SettingsModel.WriteToDisk), 
				new VisibilityConverter(false, true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeLower(SettingsModel model)
		{
			var result = Create.Grid(4, 2, true);
			result.Add(Create.Label("Use ASIO", new(0, 0)));
			result.Add(MakeAsio(model, new(0, 1)));
			result.Add(Create.Label("Device", new(1, 0)));
			result.Add(MakeAsioDevice(model, new(1, 1)));
			result.Add(MakeWasapiDevice(model, new(1, 1)));
			result.Add(Create.Label("Buffer size (ms)", new(2, 0)));
			result.Add(MakeBufferSize(model, new(2, 1)));
			result.Add(Create.Label("Format supported", new(3, 0)));
			result.Add(MakeFormatSupport(model, new(3, 1)));
			var binding = Bind.To(model, nameof(SettingsModel.WriteToDisk), 
				new VisibilityConverter(false, false));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static ComboBox MakeCombo(Cell cell)
		{
			var result = Create.Element<ComboBox>(cell);
			result.Width = ComboWidth;
			result.MinWidth = ComboWidth;
			result.MaxWidth = ComboWidth;
			result.HorizontalAlignment = HorizontalAlignment.Left;
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

		static UIElement MakeWriteToDisk(SettingsModel model, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Add(MakeWriteToDisk(model));
			result.Add(MakeBrowseOutputPath(model));
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			return result;
		}

		static UIElement MakeWriteToDisk(SettingsModel model)
		{
			var result = new CheckBox();
			var binding = Bind.To(model, nameof(model.WriteToDisk));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
			return result;
		}

		static UIElement MakeBrowseOutputPath(SettingsModel model)
		{
			var result = new Button();
			result.Content = "Browse";
			result.Click += (s, e) => BrowseOutputPath(model);
			result.HorizontalAlignment = HorizontalAlignment.Right;
			var binding = Bind.To(model, nameof(SettingsModel.WriteToDisk), 
				new VisibilityConverter(true, true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static void BrowseOutputPath(SettingsModel model)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "Raw audio (*.raw)|*.raw";
			if (dialog.ShowDialog() != true) return;
			model.OutputPath = dialog.FileName;
		}

		static UIElement MakeOutputPath(SettingsModel model, Cell cell)
		{
			var result = Create.Element<Label>(cell);
			var binding = Bind.To(model, nameof(SettingsModel.OutputPath));
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeTheme(SettingsModel model, Cell cell)
		{
			var result = MakeCombo(cell);
			result.ItemsSource = Enum.GetValues<ThemeType>();
			var binding = Bind.To(model, nameof(model.Theme));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeAsio(SettingsModel model, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Add(MakeUseAsio(model));
			result.Add(MakeAsioControlPanel(model));
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
			var result = MakeCombo(cell);
			result.ItemsSource = AudioModel.BufferSizes;
			var binding = Bind.To(model, nameof(model.BufferSize));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			result.SelectedValuePath = nameof(EnumModel<BufferSize>.Value);
			return result;
		}

		static UIElement MakeBitDepth(SettingsModel model, Cell cell)
		{
			var result = MakeCombo(cell);
			result.ItemsSource = AudioModel.BitDepths;
			var binding = Bind.To(model, nameof(model.BitDepth));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			result.SelectedValuePath = nameof(EnumModel<BitDepth>.Value);
			return result;
		}

		static UIElement MakeSampleRate(SettingsModel model, Cell cell)
		{
			var result = MakeCombo(cell);
			result.ItemsSource = AudioModel.SampleRates;
			var binding = Bind.To(model, nameof(model.SampleRate));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			result.SelectedValuePath = nameof(EnumModel<SampleRate>.Value);
			return result;
		}

		static ComboBox MakeDevice(
			SettingsModel model, bool asio, string path, Cell cell)
		{
			var result = MakeCombo(cell);
			result.SelectedValuePath = nameof(DeviceModel.Id);
			var binding = Bind.To(model, path);
			result.SetBinding(Selector.SelectedValueProperty, binding);
			binding = Bind.To(model, nameof(SettingsModel.UseAsio), 
				new VisibilityConverter(false, asio));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeAsioControlPanel(SettingsModel model)
		{
			var result = new Button();
			result.Content = "Control panel";
			result.Click += (s, e) => ShowASIOControlPanel?.Invoke(null, EventArgs.Empty);
			var binding = Bind.To(model, nameof(SettingsModel.UseAsio), 
				new VisibilityConverter(true, true));
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
			if (!args.IsSupported) return "False";
			string min = args.MinBuffer.ToString("N1");
			string max = args.MaxBuffer.ToString("N1");
			return $"True, buffer: {min} .. {max}ms";
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