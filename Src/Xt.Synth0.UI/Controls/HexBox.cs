using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Xt.Synth0.UI
{
	public class HexBox : RangeBase
	{
		public static readonly RoutedEvent OnParsedEvent = EventManager.RegisterRoutedEvent(
			   nameof(OnParsed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HexBox));

		static readonly DependencyPropertyKey HexValue1PropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(HexValue1), typeof(string), typeof(HexBox), new PropertyMetadata(0.ToString("X1")));
		public static readonly DependencyProperty HexValue1Property = HexValue1PropertyKey.DependencyProperty;
		public static string GetHexValue1(DependencyObject obj) => (string)obj.GetValue(HexValue1Property);
		static void SetHexValue1(DependencyObject obj, string value) => obj.SetValue(HexValue1PropertyKey, value);

		static readonly DependencyPropertyKey HexValue2PropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(HexValue2), typeof(string), typeof(HexBox), new PropertyMetadata(0.ToString("X1")));
		public static readonly DependencyProperty HexValue2Property = HexValue2PropertyKey.DependencyProperty;
		public static string GetHexValue2(DependencyObject obj) => (string)obj.GetValue(HexValue2Property);
		static void SetHexValue2(DependencyObject obj, string value) => obj.SetValue(HexValue2PropertyKey, value);

		static readonly DependencyPropertyKey FocusedIndexPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(FocusedIndex), typeof(int), typeof(HexBox), new PropertyMetadata(0));
		public static readonly DependencyProperty FocusedIndexProperty = FocusedIndexPropertyKey.DependencyProperty;
		public static int GetFocusedIndex(DependencyObject obj) => (int)obj.GetValue(FocusedIndexProperty);
		static void SetFocusedIndex(DependencyObject obj, int value) => obj.SetValue(FocusedIndexPropertyKey, value);

		static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) => ((HexBox)obj).Reformat();

		static HexBox()
		{
			ValueProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnValueChanged));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(typeof(HexBox)));
		}

		TextBlock Text1 => (TextBlock)Template.FindName("text1", this);
		TextBlock Text2 => (TextBlock)Template.FindName("text2", this);

		int FocusedIndex
		{
			get => GetFocusedIndex(this);
			set => SetFocusedIndex(this, value);
		}

		protected string HexValue1
		{
			get => GetHexValue1(this);
			set => SetHexValue1(this, value);
		}

		protected string HexValue2
		{
			get => GetHexValue2(this);
			set => SetHexValue2(this, value);
		}

		public event RoutedEventHandler OnParsed
		{
			add { AddHandler(OnParsedEvent, value); }
			remove { RemoveHandler(OnParsedEvent, value); }
		}

		protected virtual void Reformat()
		{
			HexValue1 = (((int)Value) & 0X0000000F).ToString("X1");
			HexValue2 = ((((int)Value) & 0X000000F0) >> 4).ToString("X1");
		}

		protected override void OnGotFocus(RoutedEventArgs e) => SetFocus(Text1);
		protected override void OnMouseDown(MouseButtonEventArgs e) => SetFocus(e.OriginalSource);
		protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e) => SetFocus(Text1);
		protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e) => SetFocus(null);

		void SetFocus(object focused)
		{
			if (focused == Text1)
			{
				FocusedIndex = 1;
				Keyboard.Focus(Text1);
				FocusManager.SetFocusedElement(this, Text1);
			}
			else if (focused == Text2)
			{
				FocusedIndex = 1;
				Keyboard.Focus(Text2);
				FocusManager.SetFocusedElement(this, Text2);
			}
			else
			{
				FocusedIndex = 0;
			}
		}

		protected override void OnTextInput(TextCompositionEventArgs e)
		{
			char c = char.ToLower(e.Text[0]);
			int value = '0' <= c && c <= '9' ? c - '0' : 'a' <= c && c <= 'f' ? c - 'a' + 10 : -1;
			if (value == -1) return;
			if (FocusedIndex == 1)
			{
				HexValue1 = value.ToString("X1");
				FocusedIndex = 2;
			}
			else if (FocusedIndex == 2)
			{
				HexValue2 = value.ToString("X1");
				RaiseEvent(new RoutedEventArgs(OnParsedEvent));
			}
		}
	}
}