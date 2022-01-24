using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Xt.Synth0.UI
{
	public class ColorBox : UserControl
	{
		public static string GetColor(DependencyObject obj) => (string)obj.GetValue(ColorProperty);
		public static void SetColor(DependencyObject obj, string value) => obj.SetValue(ColorProperty, value);
		static void OnColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
			=> ((ColorBox)obj)._box.Text = (string)e.NewValue;
		public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color),
			typeof(string), typeof(ColorBox), new FrameworkPropertyMetadata("#000000",
				FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnColorChanged)));

		public string Color
		{
			get => GetColor(this);
			set => SetColor(this, value);
		}

		readonly TextBox _box = new TextBox();
		void OnLostFocus(object sender, RoutedEventArgs e) => _box.Text = Color;

		public ColorBox()
		{
			Content = _box;
			Padding = new(0.0);
			_box.LostFocus += OnLostFocus;
			_box.TextChanged += OnTextChanged;
			_box.Margin = new(0.0, 2.0, 0.0, 2.0);
			_box.TextAlignment = TextAlignment.Left;
			_box.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
		}

		void OnTextChanged(object s, TextChangedEventArgs e)
		{
			try
			{
				ColorConverter.ConvertFromString(_box.Text);
				Color = _box.Text;
			}
			catch (FormatException)
			{
			}
		}
	}
}