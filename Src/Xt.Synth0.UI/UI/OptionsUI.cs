using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class OptionsUI
	{
		public static void Show(OptionsModel model)
		{
			var window = Create.Window(model);
			window.Title = "Options";
			window.Content = MakeContent(model);
			window.ShowDialog();
		}

		static UIElement MakeContent(OptionsModel model)
		{
			var result = Create.Grid(3, 2);
			result.Children.Add(Create.Label("Theme", new(0, 0)));
			result.Children.Add(MakeTheme(model, new(0, 1)));
			result.Children.Add(Create.Label("Use ASIO", new(1, 0)));
			result.Children.Add(MakeUseAasio(model, new(1, 1)));
			result.Children.Add(Create.Label("Sample rate", new(2, 0)));
			result.Children.Add(MakeSampleRate(model, new(2, 1)));
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
	}
}