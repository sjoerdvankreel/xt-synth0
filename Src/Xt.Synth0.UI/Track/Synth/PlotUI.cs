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
		static readonly PointCollection PlotData = new(new Point[256]);
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;
		static readonly RequestPlotDataEventArgs Args = new RequestPlotDataEventArgs();

		internal static UIElement Make(AppModel model)
		{
			var result = Create.Group("Plot");
			result.Padding = new Thickness(2.0);
			var content = new ContentControl();
			content.SizeChanged += (s, e) => Update(result, content);
			model.Track.ParamChanged += (s, e) => Update(result, content);
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
			result.VerticalAlignment = VerticalAlignment.Stretch;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.Add(PlotLine(container, data, samples));
			return result;
		}

		static UIElement PlotLine(FrameworkElement container, float[] data, int samples)
		{
			var result = new Polyline();
			result.StrokeThickness = 1;
			MapPlotData(container, data, samples);
			result.Points = PlotData;
			result.SetResourceReference(Shape.StrokeProperty, "Foreground2Key");
			return result;
		}

		static void MapPlotData(FrameworkElement container, float[] data, int samples)
		{
			for (int i = 0; i < PlotData.Count; i++)
			{
				var pos = (double)i / (PlotData.Count - 1);
				var xSample = pos * samples;
				var weight = xSample - (int)xSample;
				var x0 = (int)xSample;
				var x1 = (int)Math.Ceiling(xSample);
				var x = pos * container.ActualWidth;
				var y0 = (1.0 - weight) * data[x0];
				var y1 = weight * data[x1];
				var y = (-(y0 + y1) * 0.5 + 0.5) * container.ActualHeight;
				PlotData[i] = new Point(x, y);
			}
		}
	}
}