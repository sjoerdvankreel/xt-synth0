using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class SliderUI
	{
		internal const int ValueWidth = 32;
		internal const int SliderSize = 128;

		static void ShowEditDialog(Param<int> param)
		{
			var window = new Window();
			window.Title = $"Edit {param.Info.Name}";
			var wrap = new WrapPanel();
			wrap.Margin = new Thickness(UI.Margin);
			window.Content = wrap;
			var block = new TextBlock();
			block.Text = $"Value (min {param.Info.Min}, max {param.Info.Max}): ";
			wrap.Children.Add(block);
			var value = new TextBox();
			value.Width = ValueWidth;
			value.Text = param.Value.ToString();
			value.TextAlignment = TextAlignment.Right;
			wrap.Children.Add(value);
			var ok = new Button();
			ok.Content = "OK";
			ok.Click += (s, e) => Edit(window, param, value.Text);
			wrap.Children.Add(ok);
			window.Content = wrap;
			window.ResizeMode = ResizeMode.NoResize;
			window.Owner = Application.Current.MainWindow;
			window.SizeToContent = SizeToContent.WidthAndHeight;
			window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			window.ShowDialog();
		}

		static void Edit(Window window, Param<int> param, string value)
		{
			if (!int.TryParse(value, out int newValue)) return;
			if (newValue < param.Info.Min || newValue > param.Info.Max) return;
			param.Value = newValue;
			window.Close();
		}

		static string Format(ParamInfo<int> info, int value)
		=> info.Type switch
		{
			ParamType.Type => FormatType(value),
			ParamType.Note => UI.NoteNames[value],
			ParamType.Int => value.ToString(),
			ParamType.Float => FormatFloat(info, value),
			_ => throw new ArgumentException()
		};

		static string FormatType(int value) => value switch
		{
			TypeModel.Tri => nameof(TypeModel.Tri),
			TypeModel.Saw => nameof(TypeModel.Saw),
			TypeModel.Sine => nameof(TypeModel.Sine),
			TypeModel.Pulse => nameof(TypeModel.Pulse),
			_ => throw new ArgumentException()
		};

		static string FormatFloat(ParamInfo<int> info, int value)
		{
			double min = info.Min;
			double max = info.Max;
			return ((value - min) / (max - min)).ToString("P0").PadLeft(4, '0');
		}

		static UIElement MakeLabel(Param<int> param, int row)
		{
			var result = UI.MakeElement<Label>(row, 1);
			var binding = Bind.To(param, v => $"({Format(param.Info, v)})");
			result.SetBinding(ContentControl.ContentProperty, binding);
			result.VerticalContentAlignment = VerticalAlignment.Top;
			return result;
		}

		static UIElement MakeSlider(Param<int> param, int row)
		{
			var result = UI.MakeElement<Slider>(row, 3);
			result.Width = SliderSize;
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.VerticalAlignment = VerticalAlignment.Center;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(FrameworkElement.ToolTipProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => ShowEditDialog(param);
			return result;
		}

		internal static void Add(Grid grid, Param<int> param, int row)
		{
			grid.Children.Add(MakeSlider(param, row));
			grid.Children.Add(MakeLabel(param, row));
			grid.Children.Add(UI.MakeLabel(param.Info.Name, row, 0));
			grid.Children.Add(UI.MakeLabel(Format(param.Info, param.Info.Min), row, 2));
			grid.Children.Add(UI.MakeLabel(Format(param.Info, param.Info.Max), row, 4));
		}
	}
}