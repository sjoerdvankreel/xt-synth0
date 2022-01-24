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
		const int LeftControlWidth = 200;
		const int RightControlWidth = 80;
		public static event EventHandler ShowASIOControlPanel;
		public static event EventHandler<QueryFormatSupportEventArgs> QueryFormatSupport;

		public static void Show(SettingsModel settings)
		{
			var window = Create.Window(settings, settings.Group);
			window.Content = MakeContent(window, settings);
			window.ShowDialog();
		}

		static UIElement MakeContent(Window window, SettingsModel settings)
		{
			var result = new StackPanel();
			result.Add(MakeGroups(settings));
			result.Add(MakeOK(window));
			return result;
		}

		static UIElement MakeGroups(SettingsModel settings)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Horizontal;
			result.Add(Create.ThemedGroup(settings, settings, MakeAudioContent(settings), "Audio"));
			result.Add(Create.ThemedGroup(settings, settings, MakeThemeContent(settings), "Theme"));
			return result;
		}

		static UIElement MakeThemeContent(SettingsModel settings)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Vertical;
			result.Add(MakeThemeType(settings));
			result.Add(MakeThemeColor(settings));
			result.Add(MakeGroupColor(settings));
			result.SetValue(Grid.IsSharedSizeScopeProperty, true);
			return Create.ThemedContent(result);
		}

		static UIElement MakeThemeType(SettingsModel settings)
		{
			var result = Create.Grid(1, 2, true);
			result.Add(Create.Label("Theme type", new(0, 0)));
			result.Add(MakeThemeType(settings, new(0, 1)));
			return result;
		}

		static UIElement MakeThemeColor(SettingsModel settings)
		{
			var result = Create.Grid(1, 2, true);
			result.Add(Create.Label("Color", new(0, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.ThemeColor), new(0, 1)));
			var conv = new VisibilityConverter<ThemeType>(false, ThemeType.Themed);
			var binding = Bind.To(settings, nameof(settings.ThemeType), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeGroupColor(SettingsModel settings)
		{
			var result = Create.Grid(8, 2, true);
			result.Add(Create.Label("LFO", new(0, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.LfoColor), new(0, 1)));
			result.Add(Create.Label("Plot", new(1, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.PlotColor), new(1, 1)));
			result.Add(Create.Label("Unit", new(2, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.UnitColor), new(2, 1)));
			result.Add(Create.Label("Global", new(3, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.GlobalColor), new(3, 1)));
			result.Add(Create.Label("Pattern", new(4, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.PatternColor), new(4, 1)));
			result.Add(Create.Label("Control", new(5, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.ControlColor), new(5, 1)));
			result.Add(Create.Label("Settings", new(6, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.SettingsColor), new(6, 1)));
			result.Add(Create.Label("Envelope", new(7, 0)));
			result.Add(MakeThemeColor(settings, nameof(settings.EnvelopeColor), new(7, 1))); 
			var conv = new VisibilityConverter<ThemeType>(false, ThemeType.Grouped);
			var binding = Bind.To(settings, nameof(settings.ThemeType), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeAudioContent(SettingsModel settings)
		{
			var result = new StackPanel();
			result.Orientation = Orientation.Vertical;
			result.Add(MakeUpper(settings));
			result.Add(MakeCenter(settings));
			result.Add(MakeLower(settings));
			result.SetValue(Grid.IsSharedSizeScopeProperty, true);
			return Create.ThemedContent(result);
		}

		static UIElement MakeUpper(SettingsModel settings)
		{
			var result = Create.Grid(3, 2, true);
			result.Add(Create.Label("Bit depth", new(0, 0)));
			result.Add(MakeBitDepth(settings, new(0, 1)));
			result.Add(Create.Label("Sample rate", new(1, 0)));
			result.Add(MakeSampleRate(settings, new(1, 1)));
			result.Add(Create.Label("Write to disk", new(2, 0)));
			result.Add(MakeWriteToDisk(settings, new(2, 1)));
			return result;
		}

		static UIElement MakeCenter(SettingsModel settings)
		{
			var result = Create.Grid(1, 2, true);
			result.Add(Create.Label("Output path", new(0, 0)));
			result.Add(MakeOutputPath(settings, new(0, 1)));
			var conv = new VisibilityConverter<bool>(false, true);
			var binding = Bind.To(settings, nameof(SettingsModel.WriteToDisk), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeLower(SettingsModel settings)
		{
			var result = Create.Grid(4, 2, true);
			result.Add(Create.Label("Use ASIO", new(0, 0)));
			result.Add(MakeAsio(settings, new(0, 1)));
			result.Add(Create.Label("Device", new(1, 0)));
			result.Add(MakeAsioDevice(settings, new(1, 1)));
			result.Add(MakeWasapiDevice(settings, new(1, 1)));
			result.Add(Create.Label("Buffer size (ms)", new(2, 0)));
			result.Add(MakeBufferSize(settings, new(2, 1)));
			result.Add(Create.Label("Format supported", new(3, 0)));
			result.Add(MakeFormatSupport(settings, new(3, 1)));
			var conv = new VisibilityConverter<bool>(false, false);
			var binding = Bind.To(settings, nameof(SettingsModel.WriteToDisk), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static ComboBox MakeCombo(Cell cell, int width)
		{
			var result = Create.Element<ComboBox>(cell);
			result.Width = width;
			result.MinWidth = width;
			result.MaxWidth = width;
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

		static UIElement MakeThemeType(SettingsModel settings, Cell cell)
		{
			var result = MakeCombo(cell, RightControlWidth);
			result.ItemsSource = Enum.GetValues<ThemeType>();
			var binding = Bind.To(settings, nameof(settings.ThemeType));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			return result;
		}

		static UIElement MakeThemeColor(SettingsModel settings, string property, Cell cell)
		{
			var result = Create.Element<ColorBox>(cell);
			result.Width = RightControlWidth;
			var binding = Bind.To(settings, property);
			result.SetBinding(ColorBox.ColorProperty, binding);
			return result;
		}

		static UIElement MakeWriteToDisk(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Add(MakeWriteToDisk(settings));
			result.Add(MakeBrowseOutputPath(settings));
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			return result;
		}

		static UIElement MakeWriteToDisk(SettingsModel settings)
		{
			var result = new CheckBox();
			var binding = Bind.To(settings, nameof(settings.WriteToDisk));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
			return result;
		}

		static UIElement MakeBrowseOutputPath(SettingsModel settings)
		{
			var result = new Button();
			result.Content = "Browse";
			result.Click += (s, e) => BrowseOutputPath(settings);
			result.HorizontalAlignment = HorizontalAlignment.Right;
			var conv = new VisibilityConverter<bool>(true, true);
			var binding = Bind.To(settings, nameof(SettingsModel.WriteToDisk), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static void BrowseOutputPath(SettingsModel settings)
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "Raw audio (*.raw)|*.raw";
			if (dialog.ShowDialog() != true) return;
			settings.OutputPath = dialog.FileName;
		}

		static UIElement MakeOutputPath(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<Label>(cell);
			var binding = Bind.To(settings, nameof(SettingsModel.OutputPath));
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeAsio(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Add(MakeUseAsio(settings));
			result.Add(MakeAsioControlPanel(settings));
			return result;
		}

		static UIElement MakeUseAsio(SettingsModel settings)
		{
			var result = new CheckBox();
			var binding = Bind.To(settings, nameof(settings.UseAsio));
			result.SetBinding(ToggleButton.IsCheckedProperty, binding);
			return result;
		}

		static UIElement MakeAsioDevice(SettingsModel settings, Cell cell)
		{
			var result = MakeDevice(settings, true,
				nameof(SettingsModel.AsioDeviceId), cell);
			result.ItemsSource = AudioModel.AsioDevices;
			return result;
		}

		static UIElement MakeWasapiDevice(SettingsModel settings, Cell cell)
		{
			var result = MakeDevice(settings, false,
				nameof(SettingsModel.WasapiDeviceId), cell);
			result.ItemsSource = AudioModel.WasapiDevices;
			return result;
		}

		static UIElement MakeBufferSize(SettingsModel settings, Cell cell)
		{
			var result = MakeCombo(cell, LeftControlWidth);
			result.ItemsSource = AudioModel.BufferSizes;
			var binding = Bind.To(settings, nameof(settings.BufferSize));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			result.SelectedValuePath = nameof(EnumModel<BufferSize>.Enum);
			return result;
		}

		static UIElement MakeBitDepth(SettingsModel settings, Cell cell)
		{
			var result = MakeCombo(cell, LeftControlWidth);
			result.ItemsSource = AudioModel.BitDepths;
			var binding = Bind.To(settings, nameof(settings.BitDepth));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			result.SelectedValuePath = nameof(EnumModel<BitDepth>.Enum);
			return result;
		}

		static UIElement MakeSampleRate(SettingsModel settings, Cell cell)
		{
			var result = MakeCombo(cell, LeftControlWidth);
			result.ItemsSource = AudioModel.SampleRates;
			var binding = Bind.To(settings, nameof(settings.SampleRate));
			result.SetBinding(Selector.SelectedValueProperty, binding);
			result.SelectedValuePath = nameof(EnumModel<SampleRate>.Enum);
			return result;
		}

		static ComboBox MakeDevice(
			SettingsModel settings, bool asio, string path, Cell cell)
		{
			var result = MakeCombo(cell, LeftControlWidth);
			result.SelectedValuePath = nameof(DeviceModel.Id);
			var binding = Bind.To(settings, path);
			result.SetBinding(Selector.SelectedValueProperty, binding);
			var conv = new VisibilityConverter<bool>(false, asio);
			binding = Bind.To(settings, nameof(SettingsModel.UseAsio), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeAsioControlPanel(SettingsModel settings)
		{
			var result = new Button();
			result.Content = "Control panel";
			result.Click += (s, e) => ShowASIOControlPanel?.Invoke(null, EventArgs.Empty);
			var conv = new VisibilityConverter<bool>(true, true);
			var binding = Bind.To(settings, nameof(SettingsModel.UseAsio), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeFormatSupport(SettingsModel settings, Cell cell)
		{
			var result = Create.Element<Label>(cell);
			result.Content = DoQueryFormatSupport();
			settings.PropertyChanged += (s, e) => UpdateDeviceBuffer(result, e);
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