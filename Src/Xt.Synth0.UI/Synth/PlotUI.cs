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

		internal static UIElement Make(AppModel model)
		{
			var result = new GroupBox();
			result.Header = "Plot";
			result.Content = MakeContent(model);
			return result;
		}

		static UIElement MakeContent(AppModel model)
		{
			var result = new ContentControl();
			result.MinHeight = MinHeight;
			result.Content = Plot(result);
			result.SizeChanged += (s, e) => result.Content = Plot(result);
			model.Synth.ParamChanged += (s, e) => result.Content = Plot(result);
			model.Settings.PropertyChanged += (s, e) => result.Content = Plot(result);
			return result;
		}

		static float[] GetPlotData()
		{
			var args = new RequestPlotDataEventArgs();
			RequestPlotData?.Invoke(null, args);
			return args.Data;
		}

		static UIElement Plot(FrameworkElement container)
		{
			var result = new Canvas();
			result.Width = container.ActualWidth;
			result.Height = container.ActualHeight;
			result.Children.Add(PlotLine(container));
			return result;
		}

		static UIElement PlotLine(FrameworkElement container)
		{
			var result = new Polyline();
			result.StrokeThickness = 1;
			result.Points = MapPlotData(container, GetPlotData());
			result.SetResourceReference(Shape.StrokeProperty, "Foreground2Key");
			return result;
		}

		static PointCollection MapPlotData(FrameworkElement container, float[] data)
		{
			var result = new PointCollection();
			for (int i = 0; i < data.Length; i++)
			{
				var x = (double)i / data.Length * container.ActualWidth;
				var y = (-data[i] * 0.5 + 0.5) * container.ActualHeight;
				result.Add(new Point(x, y));
			}
			return result;
		}
	}
}