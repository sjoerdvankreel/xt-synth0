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
            var window = Create.Window(settings, settings.ThemeGroup, WindowStartupLocation.CenterScreen);
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
            result.Add(Create.ThemedGroup(settings, settings, Create.ThemedContent(MakeTheme(settings)), "Theme"));
            return result;
        }

        static FrameworkElement MakeTheme(SettingsModel settings)
        {
            var result = Create.Grid(10, 2, true);
            result.Add(Create.Label("Amp", new(0, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.AmpColor), new(0, 1)));
            result.Add(Create.Label("LFO", new(1, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.LfoColor), new(1, 1)));
            result.Add(Create.Label("Env", new(2, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.EnvColor), new(2, 1)));
            result.Add(Create.Label("Plot", new(3, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.PlotColor), new(3, 1)));
            result.Add(Create.Label("Unit", new(4, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.UnitColor), new(4, 1)));
            result.Add(Create.Label("Delay", new(5, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.DelayColor), new(5, 1)));
            result.Add(Create.Label("Filter", new(6, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.FilterColor), new(6, 1)));
            result.Add(Create.Label("Pattern", new(7, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.PatternColor), new(7, 1)));
            result.Add(Create.Label("Control", new(8, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.ControlColor), new(8, 1)));
            result.Add(Create.Label("Settings", new(9, 0)));
            result.Add(MakeThemeColor(settings, nameof(settings.SettingsColor), new(9, 1)));
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
            result.Add(Create.Label("Type", new(0, 0)));
            result.Add(MakeType(settings, new(0, 1)));
            result.Add(Create.Label("Device", new(1, 0)));
            result.Add(MakeAsioDevice(settings, new(1, 1)));
            result.Add(MakeDSoundDevice(settings, new(1, 1)));
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

        static UIElement MakeType(SettingsModel settings, Cell cell)
        {
            var result = Create.Grid(1, 2);
            result.SetValue(Grid.RowProperty, cell.Row);
            result.SetValue(Grid.ColumnProperty, cell.Col);
            result.Add(MakeDeviceType(settings, new(0, 0)));
            result.Add(MakeAsioControlPanel(settings, new(0, 1)));
            return result;
        }

        static UIElement MakeDeviceType(SettingsModel settings, Cell cell)
        {
            var result = Create.Element<ComboBox>(cell);
            result.ItemsSource = AudioIOModel.DeviceTypes;
            var binding = Bind.To(settings, nameof(settings.DeviceType));
            result.SetBinding(Selector.SelectedValueProperty, binding);
            result.SelectedValuePath = nameof(EnumModel<DeviceType>.Enum);
            return result;
        }

        static UIElement MakeAsioDevice(SettingsModel settings, Cell cell)
        {
            var result = MakeDevice(settings, DeviceType.Asio,
                nameof(SettingsModel.AsioDeviceId), cell);
            result.ItemsSource = AudioIOModel.AsioDevices;
            return result;
        }

        static UIElement MakeWasapiDevice(SettingsModel settings, Cell cell)
        {
            var result = MakeDevice(settings, DeviceType.Wasapi,
                nameof(SettingsModel.WasapiDeviceId), cell);
            result.ItemsSource = AudioIOModel.WasapiDevices;
            return result;
        }

        static UIElement MakeDSoundDevice(SettingsModel settings, Cell cell)
        {
            var result = MakeDevice(settings, DeviceType.DSound,
                nameof(SettingsModel.DSoundDeviceId), cell);
            result.ItemsSource = AudioIOModel.DSoundDevices;
            return result;
        }

        static UIElement MakeBufferSize(SettingsModel settings, Cell cell)
        {
            var result = MakeCombo(cell, LeftControlWidth);
            result.ItemsSource = AudioIOModel.BufferSizes;
            var binding = Bind.To(settings, nameof(settings.BufferSize));
            result.SetBinding(Selector.SelectedValueProperty, binding);
            result.SelectedValuePath = nameof(EnumModel<BufferSize>.Enum);
            return result;
        }

        static UIElement MakeBitDepth(SettingsModel settings, Cell cell)
        {
            var result = MakeCombo(cell, LeftControlWidth);
            result.ItemsSource = AudioIOModel.BitDepths;
            var binding = Bind.To(settings, nameof(settings.BitDepth));
            result.SetBinding(Selector.SelectedValueProperty, binding);
            result.SelectedValuePath = nameof(EnumModel<BitDepth>.Enum);
            return result;
        }

        static UIElement MakeSampleRate(SettingsModel settings, Cell cell)
        {
            var result = MakeCombo(cell, LeftControlWidth);
            result.ItemsSource = AudioIOModel.SampleRates;
            var binding = Bind.To(settings, nameof(settings.SampleRate));
            result.SetBinding(Selector.SelectedValueProperty, binding);
            result.SelectedValuePath = nameof(EnumModel<SampleRate>.Enum);
            return result;
        }

        static ComboBox MakeDevice(
            SettingsModel settings, DeviceType type, string path, Cell cell)
        {
            var result = MakeCombo(cell, LeftControlWidth);
            result.SelectedValuePath = nameof(DeviceModel.Id);
            var binding = Bind.To(settings, path);
            result.SetBinding(Selector.SelectedValueProperty, binding);
            var conv = new VisibilityConverter<DeviceType>(false, type);
            binding = Bind.To(settings, nameof(SettingsModel.DeviceType), conv);
            result.SetBinding(UIElement.VisibilityProperty, binding);
            return result;
        }

        static string DoQueryFormatSupport()
        {
            var args = new QueryFormatSupportEventArgs();
            QueryFormatSupport?.Invoke(null, args);
            if (!args.IsSupported) return "False";
            string min = args.MinBuffer.ToString("N1");
            string max = args.MaxBuffer.ToString("N1");
            return $"True, {min} .. {max}ms";
        }

        static UIElement MakeFormatSupport(SettingsModel settings, Cell cell)
        {
            var result = Create.Element<Label>(cell);
            result.Content = DoQueryFormatSupport();
            settings.PropertyChanged += (s, e) => UpdateDeviceBuffer(result, e);
            return result;
        }

        static void UpdateDeviceBuffer(Label label, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsModel.BitDepth) ||
                e.PropertyName == nameof(SettingsModel.SampleRate) ||
                e.PropertyName == nameof(SettingsModel.DeviceType) ||
                e.PropertyName == nameof(SettingsModel.AsioDeviceId) ||
                e.PropertyName == nameof(SettingsModel.DSoundDeviceId) ||
                e.PropertyName == nameof(SettingsModel.WasapiDeviceId))
                label.Content = DoQueryFormatSupport();
        }

        static UIElement MakeAsioControlPanel(SettingsModel settings, Cell cell)
        {
            var result = Create.Element<Button>(cell);
            result.Content = "Control panel";
            result.Click += (s, e) => ShowASIOControlPanel?.Invoke(null, EventArgs.Empty);
            var conv = new VisibilityConverter<DeviceType>(true, DeviceType.Asio);
            var binding = Bind.To(settings, nameof(SettingsModel.DeviceType), conv);
            result.SetBinding(UIElement.VisibilityProperty, binding);
            return result;
        }
    }
}