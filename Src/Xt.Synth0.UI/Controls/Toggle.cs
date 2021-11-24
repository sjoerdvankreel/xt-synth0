using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Xt.Synth0.UI
{
	public class Toggle : ToggleButton
	{
		const double DefaultSize = 16.0;

		public static readonly DependencyProperty OnFillProperty = DependencyProperty.Register(
			nameof(OnFill), typeof(Brush), typeof(Toggle), new(Brushes.Gray, OnColorsChanged));
		public static Brush GetOnFill(DependencyObject obj) => (Brush)obj.GetValue(OnFillProperty);
		public static void SetOnFill(DependencyObject obj, Brush value) => obj.SetValue(OnFillProperty, value);

		public static readonly DependencyProperty OffFillProperty = DependencyProperty.Register(
			nameof(OffFill), typeof(Brush), typeof(Toggle), new(Brushes.LightGray, OnColorsChanged));
		public static Brush GetOffFill(DependencyObject obj) => (Brush)obj.GetValue(OffFillProperty);
		public static void SetOffFill(DependencyObject obj, Brush value) => obj.SetValue(OffFillProperty, value);

		public static readonly DependencyProperty OnStrokeProperty = DependencyProperty.Register(
			nameof(OnStroke), typeof(Brush), typeof(Toggle), new(Brushes.Black, OnColorsChanged));
		public static Brush GetOnStroke(DependencyObject obj) => (Brush)obj.GetValue(OnStrokeProperty);
		public static void SetOnStroke(DependencyObject obj, Brush value) => obj.SetValue(OnStrokeProperty, value);

		public static readonly DependencyProperty OffStrokeProperty = DependencyProperty.Register(
			nameof(OffStroke), typeof(Brush), typeof(Toggle), new(Brushes.Black, OnColorsChanged));
		public static Brush GetOffStroke(DependencyObject obj) => (Brush)obj.GetValue(OffStrokeProperty);
		public static void SetOffStroke(DependencyObject obj, Brush value) => obj.SetValue(OffStrokeProperty, value);

		static readonly DependencyPropertyKey EffectiveFillPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(EffectiveFill), typeof(Brush), typeof(Toggle), new(Brushes.LightGray));
		public static readonly DependencyProperty EffectiveFillProperty = EffectiveFillPropertyKey.DependencyProperty;
		public static Brush GetEffectiveFill(DependencyObject obj) => (Brush)obj.GetValue(EffectiveFillProperty);
		static void SetEffectiveFill(DependencyObject obj, Brush value) => obj.SetValue(EffectiveFillPropertyKey, value);

		static readonly DependencyPropertyKey EffectiveStrokePropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(EffectiveStroke), typeof(Brush), typeof(Toggle), new(Brushes.Black));
		public static readonly DependencyProperty EffectiveStrokeProperty = EffectiveStrokePropertyKey.DependencyProperty;
		public static Brush GetEffectiveStroke(DependencyObject obj) => (Brush)obj.GetValue(EffectiveStrokeProperty);
		static void SetEffectiveStroke(DependencyObject obj, Brush value) => obj.SetValue(EffectiveStrokePropertyKey, value);

		static readonly DependencyPropertyKey EffectiveSizePropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(EffectiveSize), typeof(double), typeof(Toggle), new(DefaultSize));
		public static readonly DependencyProperty EffectiveSizeProperty = EffectiveSizePropertyKey.DependencyProperty;
		public static double GetEffectiveSize(DependencyObject obj) => (double)obj.GetValue(EffectiveSizeProperty);
		static void SetEffectiveSize(DependencyObject obj, double value) => obj.SetValue(EffectiveSizePropertyKey, value);

		static void OnSizeChanged(object obj, RoutedEventArgs e)
		{
			var toggle = (Toggle)obj;
			var effectiveSize = Math.Min(toggle.ActualWidth, toggle.ActualHeight);
			toggle.SetValue(EffectiveSizePropertyKey, effectiveSize);
		}

		static void OnColorsChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var toggle = (Toggle)obj;
			toggle.EffectiveFill = toggle.IsChecked == true ? toggle.OnFill : toggle.OffFill;
			toggle.EffectiveStroke = toggle.IsChecked == true ? toggle.OnStroke : toggle.OffStroke;
		}

		static Toggle()
		{
			WidthProperty.OverrideMetadata(typeof(Toggle), new FrameworkPropertyMetadata(DefaultSize));
			HeightProperty.OverrideMetadata(typeof(Toggle), new FrameworkPropertyMetadata(DefaultSize));
			IsCheckedProperty.OverrideMetadata(typeof(Toggle), new FrameworkPropertyMetadata(OnColorsChanged));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Toggle), new FrameworkPropertyMetadata(typeof(Toggle)));
			EventManager.RegisterClassHandler(typeof(Toggle), SizeChangedEvent, new RoutedEventHandler(OnSizeChanged), true);
		}

		public Brush OnFill
		{
			get => GetOnFill(this);
			set => SetOnFill(this, value);
		}

		public Brush OffFill
		{
			get => GetOffFill(this);
			set => SetOffFill(this, value);
		}

		public Brush OnStroke
		{
			get => GetOnStroke(this);
			set => SetOnStroke(this, value);
		}

		public Brush OffStroke
		{
			get => GetOffStroke(this);
			set => SetOffStroke(this, value);
		}

		public Brush EffectiveFill
		{
			get => GetEffectiveFill(this);
			set => SetEffectiveFill(this, value);
		}

		public Brush EffectiveStroke
		{
			get => GetEffectiveStroke(this);
			set => SetEffectiveStroke(this, value);
		}

		public double EffectiveSize
		{
			get => GetEffectiveSize(this);
			private set => SetEffectiveSize(this, value);
		}
	}
}