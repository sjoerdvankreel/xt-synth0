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
		const int PadLeft = 20;
		const int PadBottom = 20;
		const double MaxLevel = 0.99;
		static readonly UIElement Off;
		static readonly RequestPlotDataEventArgs Args = new();
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;

		static PlotUI()
		{
			var dock = new DockPanel();
			var label = dock.Add(Create.Label("Plot OFF"));
			label.FontWeight = FontWeights.Bold;
			label.VerticalAlignment = VerticalAlignment.Center;
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.SetResourceReference(Control.ForegroundProperty, Utility.RowDisabledKey);
			Off = Create.ThemedContent(dock);
		}

		internal static UIElement Make(AppModel app)
		{
			var text = new TextBlock();
			var plot = app.Track.Synth.Plot;
			text.VerticalAlignment = VerticalAlignment.Center;
			return Create.ThemedGroup(app.Settings, plot, MakeContent(app, text), text);
		}

		static UIElement MakeContent(AppModel app, TextBlock text)
		{
			var result = new DockPanel();
			result.Add(GroupUI.MakeContent(app, app.Track.Synth.Plot), Dock.Top);
			result.Add(MakePlotContent(app, text), Dock.Top);
			return result;
		}

		static UIElement MakePlotContent(AppModel app, TextBlock text)
		{
			var synth = app.Track.Synth;
			var dock = new DockPanel();
			var content = dock.Add(new ContentControl());
			var result = Create.ThemedContent(dock);
			synth.ParamChanged += (s, e) => Update(app, text, content);
			content.SizeChanged += (s, e) => Update(app, text, content);
			app.Settings.PropertyChanged += (s, e) => Update(app, text, content);
			app.Track.Seq.Edit.Bpm.PropertyChanged += (s, e) => Update(app, text, content);
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			result.BorderThickness = new(GroupUI.BorderThickness, 0.0, GroupUI.BorderThickness, GroupUI.BorderThickness);
			return result;
		}

		static PointCollection PlotData(int w, double h, float min, float max)
		{
			double hPad = h - PadBottom;
			var result = new PointCollection();
			for (int i = 0; i < Args.Samples.Count; i++)
			{
				var samplePos = (double)i / Args.Samples.Count;
				var xSample = samplePos * Args.Samples.Count;
				var weight = xSample - (int)xSample;
				var x1 = (int)Math.Ceiling(xSample);
				var y0 = (1.0 - weight) * Args.Samples[(int)xSample];
				var y1 = weight * Args.Samples[x1];
				var y = (1.0f - MaxLevel) * hPad + (1.0 - (y0 + y1)) * MaxLevel * hPad;
				var screenPos = i / (Args.Samples.Count - 1.0);
				double l = PadLeft + screenPos * (w - PadLeft);
				result.Add(new Point(l, y / (max - min)));
			}
			return result;
		}

		static void PlotProperties(Shape shape)
		{
			shape.Opacity = 0.67;
			shape.SetResourceReference(Shape.StrokeProperty, Utility.Foreground1Key);
		}

		static UIElement PlotLine(PointCollection data)
		{
			var result = new Polyline();
			result.Points = data;
			result.StrokeThickness = 1.5;
			PlotProperties(result);
			return result;
		}

		static UIElement Marker(double x, double y, string text)
		{
			var result = Create.Text(text);
			Canvas.SetLeft(result, x);
			Canvas.SetTop(result, y);
			return result;
		}

		static UIElement PlotBar(Point p, double h, double stroke)
		{
			var result = new Line();
			result.X1 = p.X;
			result.X2 = p.X;
			result.Y2 = p.Y;
			result.Y1 = h - PadBottom;
			result.StrokeThickness = stroke;
			PlotProperties(result);
			return result;
		}

		static UIElement Split(double x1, double x2, double y1, double y2)
		{
			var result = new Line();
			result.X1 = x1;
			result.X2 = x2;
			result.Y1 = y1;
			result.Y2 = y2;
			result.Opacity = 0.5f;
			result.StrokeDashArray = new DoubleCollection(new[] { 4.0, 2.0 });
			result.SetResourceReference(Shape.StrokeProperty, Utility.Foreground2Key);
			return result;
		}

		static void Update(AppModel app, TextBlock text, ContentControl container)
		{
			var plot = app.Track.Synth.Plot;
			int w = (int)container.ActualWidth;
			double h = container.ActualHeight;
			Args.Pixels = w;
			RequestPlotData?.Invoke(null, Args);
			container.Content = Args.Samples.Count > 0 ? Plot(w, h, Args.Min, Args.Max) : Off;
			string header = $"{plot.Name} @ {Args.SampleRate.ToString("N1")}Hz";
			header += $"{Environment.NewLine}{Args.Samples.Count} samples";
			if (Args.Freq != 0.0f) header += $" @ {Args.Freq.ToString("N1")}Hz";
			if (Args.Clip) header += " (Clip)";
			text.Text = Args.Samples.Count > 0 ? header : $"{plot.Name}{Environment.NewLine}No data";
		}

		static UIElement Plot(int w, double h, float min, float max)
		{
			var result = new Canvas();
			var data = PlotData(w, h, min, max);
			if (!Args.Spectrum)
				result.Add(PlotLine(data));
			else
				for (int i = 0; i < data.Count; i++)
					result.Add(PlotBar(data[i], h, (double)w / data.Count));
			for (int i = 0; i < Args.VSplitVals.Count; i++)
			{
				double pos = (Args.VSplitVals[i] - min) / (max - min);
				result.Add(Split(0, w, pos * h, pos * h));
				result.Add(Marker(0, pos * h, Args.VSplitMarkers[i]));
			}
			for (int i = 0; i < Args.HSplitVals.Count; i++)
			{
				double pos = Args.HSplitVals[i] / (Args.Samples.Count - 1.0);
				result.Add(Split(pos * w, pos * w, 0, h));
				result.Add(Marker(pos * w, h, Args.HSplitMarkers[i]));
			}
			result.VerticalAlignment = VerticalAlignment.Stretch;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			return result;
		}
	}
}