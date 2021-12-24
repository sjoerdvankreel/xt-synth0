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
		const int Width = 100;
		const int Height = 50;
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;

		internal static UIElement Make(AppModel model)
		{
			var result = new GroupBox();
			result.Header = "Plot";
			result.Content = Plot();
			model.Synth.ParamChanged += (s, e) => result.Content = Plot();
			model.Settings.PropertyChanged += (s, e) => result.Content = Plot();
			return result;
		}

		static UIElement Plot()
		{
			var result = new Canvas();
			result.Width = Width;
			result.Height = Height;
			result.Children.Add(PlotLine());
			return result;
		}

		static float[] GetPlotData()
		{
			var args = new RequestPlotDataEventArgs();
			RequestPlotData?.Invoke(null, args);
			return args.Data;
		}

		static UIElement PlotLine()
		{
			var result = new Polyline();
			result.StrokeThickness = 1;
			result.Stroke = Brushes.White;
			result.Points = MapPlotData(GetPlotData());
			return result;
		}

		static PointCollection MapPlotData(float[] data)
		{
			var result = new PointCollection();
			for (int i = 0; i < data.Length; i++)
			{
				var x = (double)i / data.Length * Width;
				var y = (-data[i] * 0.5 + 0.5) * Height;
				result.Add(new Point(x, y));
			}
			return result;
		}
	}
}