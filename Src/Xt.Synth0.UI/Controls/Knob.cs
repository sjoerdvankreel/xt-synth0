using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Xt.Synth0.UI
{
	public class Knob : RangeBase
	{
		const double MinAngle = 0.05;
		const double MaxAngle = 0.95;
		const double DefaultMarkerSize = 6.0;
		const double DefaultRotarySize = 16.0;
		public const double DefaultSensitivity = 50.0;

		static Knob ActiveKnob;
		static Point? ActivePosition;
		static double? ActiveBaseValue;

		public static readonly DependencyProperty IsMixProperty = DependencyProperty.Register(
			nameof(IsMix), typeof(bool), typeof(Knob), new(false));
		public static bool GetIsMix(DependencyObject obj) => (bool)obj.GetValue(IsMixProperty);
		public static void SetIsMix(DependencyObject obj, bool value) => obj.SetValue(IsMixProperty, value);

		static readonly DependencyPropertyKey MarkerXPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(MarkerX), typeof(double), typeof(Knob), new(0.0));
		public static readonly DependencyProperty MarkerXProperty = MarkerXPropertyKey.DependencyProperty;
		public static double GetMarkerX(DependencyObject obj) => (double)obj.GetValue(MarkerXProperty);
		static void SetMarkerX(DependencyObject obj, double value) => obj.SetValue(MarkerXPropertyKey, value);

		static readonly DependencyPropertyKey MarkerYPropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(MarkerY), typeof(double), typeof(Knob), new(0.0));
		public static readonly DependencyProperty MarkerYProperty = MarkerYPropertyKey.DependencyProperty;
		public static double GetMarkerY(DependencyObject obj) => (double)obj.GetValue(MarkerYProperty);
		static void SetMarkerY(DependencyObject obj, double value) => obj.SetValue(MarkerYPropertyKey, value);

		static readonly DependencyPropertyKey EffectiveSizePropertyKey = DependencyProperty.RegisterReadOnly(
			nameof(EffectiveSize), typeof(double), typeof(Knob), new(OnMarkerPositionChanged));
		public static readonly DependencyProperty EffectiveSizeProperty = EffectiveSizePropertyKey.DependencyProperty;
		public static double GetEffectiveSize(DependencyObject obj) => (double)obj.GetValue(EffectiveSizeProperty);
		static void SetEffectiveSize(DependencyObject obj, double value) => obj.SetValue(EffectiveSizePropertyKey, value);

		public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register(
			nameof(MarkerSize), typeof(double), typeof(Knob), new(DefaultMarkerSize, OnMarkerPositionChanged));
		public static double GetMarkerSize(DependencyObject obj) => (double)obj.GetValue(MarkerSizeProperty);
		public static void SetMarkerSize(DependencyObject obj, double value) => obj.SetValue(MarkerSizeProperty, value);

		public static readonly DependencyProperty MarkerFillProperty = DependencyProperty.Register(
			nameof(MarkerFill), typeof(Brush), typeof(Knob), new(Brushes.Black));
		public static Brush GetMarkerFill(DependencyObject obj) => (Brush)obj.GetValue(MarkerFillProperty);
		public static void SetMarkerFill(DependencyObject obj, Brush value) => obj.SetValue(MarkerFillProperty, value);

		public static readonly DependencyProperty MarkerStrokeProperty = DependencyProperty.Register(
			nameof(MarkerStroke), typeof(Brush), typeof(Knob), new(Brushes.Black));
		public static Brush GetMarkerStroke(DependencyObject obj) => (Brush)obj.GetValue(MarkerStrokeProperty);
		public static void SetMarkerStroke(DependencyObject obj, Brush value) => obj.SetValue(MarkerStrokeProperty, value);

		public static readonly DependencyProperty RotaryFillProperty = DependencyProperty.Register(
			nameof(RotaryFill), typeof(Brush), typeof(Knob), new(Brushes.Gray));
		public static Brush GetRotaryFill(DependencyObject obj) => (Brush)obj.GetValue(RotaryFillProperty);
		public static void SetRotaryFill(DependencyObject obj, Brush value) => obj.SetValue(RotaryFillProperty, value);

		public static readonly DependencyProperty RotaryStrokeProperty = DependencyProperty.Register(
			nameof(RotaryStroke), typeof(Brush), typeof(Knob), new(Brushes.Black));
		public static Brush GetRotaryStroke(DependencyObject obj) => (Brush)obj.GetValue(RotaryStrokeProperty);
		public static void SetRotaryStroke(DependencyObject obj, Brush value) => obj.SetValue(RotaryStrokeProperty, value);

		public static readonly DependencyProperty SensitivityProperty = DependencyProperty.Register(
			nameof(Sensitivity), typeof(double), typeof(Knob), new(DefaultSensitivity));
		public static double GetSensitivity(DependencyObject obj) => (double)obj.GetValue(SensitivityProperty);
		public static void SetSensitivity(DependencyObject obj, double value) => obj.SetValue(SensitivityProperty, value);

		static Knob FindAncestorKnob(DependencyObject obj)
		{
			var result = obj;
			if (obj == null) return null;
			while (result is Visual && !(result is Knob))
				result = VisualTreeHelper.GetParent(result);
			return result as Knob;
		}

		static void OnSizeChanged(object obj, RoutedEventArgs e)
		{
			var knob = (Knob)obj;
			knob.EffectiveSize = Math.Min(knob.ActualWidth, knob.ActualHeight);
		}

		static void OnWindowMouseMove(object sender, MouseEventArgs e)
		{
			if (ActiveKnob == null) return;
			if (e.LeftButton != MouseButtonState.Pressed) return;
			var min = ActiveKnob.Minimum;
			var max = ActiveKnob.Maximum;
			var range = max - min;
			var pos = e.GetPosition(Application.Current.MainWindow);
			var diff = (ActivePosition.Value.Y - pos.Y) / ActiveKnob.Sensitivity;
			var newValue = ActiveBaseValue.Value + diff * range;
			ActiveKnob.Value = Math.Clamp(newValue, min, max);
		}

		static void OnWindowMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			ActiveKnob?.ReleaseMouseCapture();
			ActiveKnob = null;
			ActivePosition = null;
			ActiveBaseValue = null;
		}

		static void OnWindowMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var knob = FindAncestorKnob(e.OriginalSource as DependencyObject);
			if (knob == null) return;
			ActiveKnob = knob;
			ActiveBaseValue = knob.Value;
			ActivePosition = e.GetPosition(Application.Current.MainWindow);
			if (!ActiveKnob.CaptureMouse()) throw new InvalidOperationException();
		}

		static void OnMarkerPositionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var knob = (Knob)obj;
			var markerRadius = knob.MarkerSize / 2.0;
			var rotaryRadius = knob.EffectiveSize / 2.0;
			if (rotaryRadius == 0.0 || markerRadius == 0.0)
				return;
			var angle = (knob.Value - knob.Minimum) / (knob.Maximum - knob.Minimum);
			var theta = Math.PI * 2.0 * -(MinAngle + angle * (MaxAngle - MinAngle));
			knob.MarkerX = rotaryRadius * Math.Sin(theta) + rotaryRadius - markerRadius;
			knob.MarkerY = rotaryRadius * Math.Cos(theta) + rotaryRadius - markerRadius;
		}

		static Knob()
		{
			WidthProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(DefaultRotarySize));
			HeightProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(DefaultRotarySize));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(typeof(Knob)));
			ValueProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(OnMarkerPositionChanged));
			MinimumProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(OnMarkerPositionChanged));
			MaximumProperty.OverrideMetadata(typeof(Knob), new FrameworkPropertyMetadata(OnMarkerPositionChanged));
			EventManager.RegisterClassHandler(typeof(Knob), SizeChangedEvent, new RoutedEventHandler(OnSizeChanged), true);
			EventManager.RegisterClassHandler(typeof(Window), MouseMoveEvent, new MouseEventHandler(OnWindowMouseMove), true);
			EventManager.RegisterClassHandler(typeof(Window), MouseLeftButtonUpEvent, new MouseButtonEventHandler(OnWindowMouseLeftButtonUp), true);
			EventManager.RegisterClassHandler(typeof(Window), MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnWindowMouseLeftButtonDown), true);
		}

		public bool IsMix
		{
			get => GetIsMix(this);
			set => SetIsMix(this, value);
		}

		public double Sensitivity
		{
			get => GetSensitivity(this);
			set => SetSensitivity(this, value);
		}

		public double MarkerSize
		{
			get => GetMarkerSize(this);
			set => SetMarkerSize(this, value);
		}

		public Brush MarkerFill
		{
			get => GetMarkerFill(this);
			set => SetMarkerFill(this, value);
		}

		public Brush MarkerStroke
		{
			get => GetMarkerStroke(this);
			set => SetMarkerStroke(this, value);
		}

		public Brush RotaryFill
		{
			get => GetRotaryFill(this);
			set => SetRotaryFill(this, value);
		}

		public Brush RotaryStroke
		{
			get => GetRotaryStroke(this);
			set => SetRotaryStroke(this, value);
		}

		public double MarkerX
		{
			get => GetMarkerX(this);
			private set => SetMarkerX(this, value);
		}

		public double MarkerY
		{
			get => GetMarkerY(this);
			private set => SetMarkerY(this, value);
		}

		public double EffectiveSize
		{
			get => GetEffectiveSize(this);
			private set => SetEffectiveSize(this, value);
		}
	}
}