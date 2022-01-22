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
		const double MaxLevelBi = 0.975;
		const double MaxLevelUni = 0.99;
		static readonly DockPanel Empty;
		static readonly RequestPlotDataEventArgs Args = new();
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;

		static PlotUI()
		{
			Empty = new DockPanel();
			var label = Empty.Add(Create.Label("No data"));
			label.VerticalAlignment = VerticalAlignment.Center;
			label.HorizontalAlignment = HorizontalAlignment.Center;
			Empty.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
		}

		internal static UIElement Make(AppModel app)
		{
			var plot = app.Track.Synth.Plot;
			var result = Create.ThemedGroup(app.Settings, plot, null);
			result.Content = MakeContent(app, result);
			result.Padding = new(Padding);
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement MakeContent(AppModel app, GroupBox box)
		{
			var synth = app.Track.Synth;
			var result = new DockPanel();
			result.Add(SubUI.MakeContent(app, synth.Plot), Dock.Top);
			var content = new ContentControl();
			var border = result.Add(SubUI.MakeOuterBorder(content), Dock.Top);
			synth.ParamChanged += (s, e) => Update(app, box, content);
			content.SizeChanged += (s, e) => Update(app, box, content);
			app.Settings.PropertyChanged += (s, e) => Update(app, box, content);
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
				var y = (1.0f - MaxLevelUni) * h + (1.0 - (y0 + y1)) * MaxLevelUni * h;
				if (Args.Bipolar) y = (-(y0 + y1) * MaxLevelBi / 2.0f + 0.5) * h;
				var screenPos = i / (Args.Samples.Count - 1.0);
				result.Add(new Point(screenPos * w, y));
			}
			return result;
		}

		static UIElement PlotLine(int w, double h)
		{
			var result = new Polyline();
			result.StrokeThickness = 1.5;
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

		static void Update(AppModel app, GroupBox box, ContentControl container)
		{
			if (app.Stream.IsRunning) return;
			var plot = app.Track.Synth.Plot;
			int w = (int)container.ActualWidth;
			double h = container.ActualHeight;
			Args.Pixels = w;
			RequestPlotData?.Invoke(null, Args);
			container.Content = Args.Samples.Count > 0 ? Plot(w, h) : Empty;
			string header = $"{plot.Name} @ {Args.SampleRate.ToString("N1")}Hz";
			header += $"{Environment.NewLine}{Args.Samples.Count} samples";
			if (Args.Freq != 0.0f) header += $" @ {Args.Freq.ToString("N1")}Hz";
			if (Args.Clip) header += " (Clip)";
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