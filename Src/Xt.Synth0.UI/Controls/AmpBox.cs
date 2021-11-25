using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Xt.Synth0.UI
{
	public class AmpBox : RangeBase
	{
		public static readonly RoutedEvent OnParsedEvent = EventManager.RegisterRoutedEvent(
			nameof(OnParsed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AmpBox));

		static readonly DependencyPropertyKey HexValuePropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(HexValue), typeof(string), typeof(AmpBox), new PropertyMetadata(0.ToString("X2")));
		public static readonly DependencyProperty HexValueProperty = HexValuePropertyKey.DependencyProperty;
		public static string GetHexValue(DependencyObject obj) => (string)obj.GetValue(HexValueProperty);
		static void SetHexValue(DependencyObject obj, string value) => obj.SetValue(HexValuePropertyKey, value);

		static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var box = (AmpBox)obj;
			box.HexValue = ((int)box.Value).ToString("X2");
		}

		static AmpBox()
		{
			ValueProperty.OverrideMetadata(typeof(AmpBox), new FrameworkPropertyMetadata(OnValueChanged));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AmpBox), new FrameworkPropertyMetadata(typeof(AmpBox)));
		}

		char? _previous;

		public string HexValue
		{
			get => GetHexValue(this);
			set => SetHexValue(this, value);
		}

		public event RoutedEventHandler OnParsed
		{
			add { AddHandler(OnParsedEvent, value); }
			remove { RemoveHandler(OnParsedEvent, value); }
		}

		protected override void OnGotFocus(RoutedEventArgs e)
		{
			base.OnGotFocus(e);
			_previous = null;
		}

		protected override void OnLostFocus(RoutedEventArgs e)
		{
			base.OnLostFocus(e);
			_previous = null;
		}

		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnGotKeyboardFocus(e);
			_previous = null;
		}

		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
		{
			base.OnLostKeyboardFocus(e);
			_previous = null;
		}

		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			base.OnTextInput(e);
			char next = e.Text.FirstOrDefault();
			if (_previous == null)
			{
				_previous = next;
				return;
			}
			string text = new string(new[] { _previous.Value, next });
			if (!int.TryParse(text, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out var value))
			{
				_previous = next;
				return;
			}
			if (value < Minimum || value > Maximum)
			{
				_previous = next;
				return;
			}
			Value = value;
			_previous = null;
			RaiseEvent(new RoutedEventArgs(OnParsedEvent));
		}
	}
}