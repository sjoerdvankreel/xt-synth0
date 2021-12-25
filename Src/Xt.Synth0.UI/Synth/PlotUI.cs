using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class PlotUI
	{
		const int MinHeight = 24;
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;
		static readonly RequestPlotDataEventArgs Args = new RequestPlotDataEventArgs();

		internal static UIElement Make(AppModel model)
		{
			var result = new GroupBox();
			result.Header = "Plot";
			var content = new ContentControl();
			content.MinHeight = MinHeight;
			content.SizeChanged += (s, e) => Update(result, content);
			model.Synth.ParamChanged += (s, e) => Update(result, content);
			model.Settings.PropertyChanged += (s, e) => Update(result, content);
			result.Content = content;
			return result;
		}

		static void Update(GroupBox box, ContentControl container)
		{
			RequestPlotData?.Invoke(null, Args);
			container.Content = Plot(container, Args.Data, Args.Samples);
			var freq = Args.Frequency.ToString("N1");
			box.Header = $"Plot ({Args.Samples} @ {freq}Hz)";
		}

		static UIElement Plot(FrameworkElement container, float[] data, int samples)
		{
			var result = new Canvas();
			result.Width = container.ActualWidth;
			result.Height = container.ActualHeight;
			result.Children.Add(PlotLine(container, data, samples));
			return result;
		}

		static UIElement PlotLine(FrameworkElement container, float[] data, int samples)
		{
			var result = new Polyline();
			result.StrokeThickness = 1;
			result.Points = MapPlotData(container, data, samples);
			result.SetResourceReference(Shape.StrokeProperty, "Foreground2Key");
			return result;
		}

		static PointCollection MapPlotData(FrameworkElement container, float[] data, int samples)
		{
			var result = new PointCollection();
			for (int i = 0; i < samples; i++)
			{
				var y = (-data[i] * 0.5 + 0.5) * container.ActualHeight;
				var x = (double)i / (samples - 1) * container.ActualWidth;
				result.Add(new Point(x, y));
			}
			return result;
		}
	}
}