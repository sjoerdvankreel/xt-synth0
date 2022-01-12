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
		const double Padding = 2.0;
		static readonly RequestPlotDataEventArgs Args = new();
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;

		internal static UIElement Make(AppModel app)
		{
			var plot = app.Track.Synth.Plot;
			var result = Create.ThemedGroup(app.Settings, plot, null);
			result.Content = MakeContent(app, result);
			result.Padding = new(Padding);
			return result;
		}

		static UIElement MakeContent(AppModel app, GroupBox box)
		{
			var synth = app.Track.Synth;
			var result = new DockPanel();
			result.Add(SubUI.MakeContent(app, synth.Plot), Dock.Top);
			var content = new ContentControl();
			var border = result.Add(SubUI.MakeOuterBorder(content), Dock.Top);
			synth.ParamChanged += (s, e) => Update(synth.Plot, box, content);
			content.SizeChanged += (s, e) => Update(synth.Plot, box, content);
			app.Settings.PropertyChanged += (s, e) => Update(synth.Plot, box, content);
			border.BorderThickness = new(SubUI.BorderThickness, 0, SubUI.BorderThickness, SubUI.BorderThickness);
			return result;
		}

		static PointCollection PlotData(int w, double h)
		{
			var result = new PointCollection();
			for (int i = 0; i < Args.Samples.Count; i++)
			{
				var samplePos = (double)i / Args.Samples.Count;
				var xSample = samplePos * Args.Samples.Count;
				var weight = xSample - (int)xSample;
				var x1 = (int)Math.Ceiling(xSample);
				var y0 = (1.0 - weight) * Args.Samples[(int)xSample];
				var y1 = weight * Args.Samples[x1];
				var y = (1.0 - (y0 + y1)) * h;
				if (Args.Bipolar) y = (-(y0 + y1) * 0.5 + 0.5) * h;
				var screenPos = i / (Args.Samples.Count - 1.0);
				result.Add(new Point(screenPos * w, y));
			}
			return result;
		}

		static UIElement PlotLine(int w, double h)
		{
			var result = new Polyline();
			result.StrokeThickness = 1;
			result.Points = PlotData(w, h);
			result.SetResourceReference(Shape.StrokeProperty, Utility.Foreground1Key);
			return result;
		}

		static UIElement Marker(double x1, double x2, double y1, double y2)
		{
			var result = new Line();
			result.X1 = x1;
			result.X2 = x2;
			result.Y1 = y1;
			result.Y2 = y2;
			result.StrokeDashArray = new DoubleCollection(new[] { 4.0, 2.0 });
			result.SetResourceReference(Shape.StrokeProperty, Utility.Foreground2Key);
			return result;
		}

		static void Update(PlotModel plot, GroupBox box, ContentControl container)
		{
			int w = (int)container.ActualWidth;
			double h = container.ActualHeight;
			Args.Pixels = w;
			RequestPlotData?.Invoke(null, Args);
			container.Content = Plot(w, h);
			string header = $"{plot.Name} @ {Args.SampleRate}Hz";
			header += $"{Environment.NewLine}{Args.Samples.Count} samples";
			if (Args.Frequency != 0.0f) header += $", {Args.Frequency.ToString("N1")}Hz";
			box.Header = header;
		}

		static UIElement Plot(int w, double h)
		{
			var result = new Canvas();
			result.Add(PlotLine(w, h));
			if (Args.Bipolar)
				result.Add(Marker(0, w, h / 2.0, h / 2.0));
			for (int i = 0; i < Args.Splits.Count; i++)
			{
				double pos = Args.Splits[i] / (Args.Samples.Count - 1.0);
				result.Add(Marker(pos * w, pos * w, 0, h));
			}
			result.VerticalAlignment = VerticalAlignment.Stretch;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}
	}
}