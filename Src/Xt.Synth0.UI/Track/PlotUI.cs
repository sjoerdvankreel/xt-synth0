using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class PlotUI
	{
		static string _name;
		static GroupBox _box;
		static ContentControl _container;

		static ulong _plotRequest = 0;
		static readonly object _lock = new object();

		const double Padding = 2.0;
		const double MaxLevelBi = 0.975;
		const double MaxLevelUni = 0.99;
		static readonly DockPanel Empty;
		public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;

		static PlotUI()
		{
			Empty = new DockPanel();
			var label = Empty.Add(Create.Label("No data"));
			label.VerticalAlignment = VerticalAlignment.Center;
			label.HorizontalAlignment = HorizontalAlignment.Center;
			Empty.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);

			var thread = new Thread(PlotLoop);
			thread.IsBackground = true;
			thread.Start();
		}

		static void BeginUpdate(object sender, EventArgs e)
		{
			ulong request = _plotRequest;
			Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
			{
				lock (_lock)
				{
					if (request == _plotRequest)
					{
						_plotRequest++;
						Monitor.Pulse(_lock);
					}
				}
			}), DispatcherPriority.Background);
		}

		static void PlotLoop(object arg)
		{
			ulong request = 0;
			while (true)
			{
				lock (_lock)
				{
					while (request == _plotRequest)
						Monitor.Wait(_lock);
					request = _plotRequest;
				}
				var args = new RequestPlotDataEventArgs();
				args.Pixels = (int)_container.ActualWidth;
				RequestPlotData?.Invoke(null, args);
				var action = new Action(() => Update(args));
				Application.Current.Dispatcher.Invoke(action, DispatcherPriority.ApplicationIdle);
			}
		}

		internal static UIElement Make(AppModel app)
		{
			var plot = app.Track.Synth.Plot;
			_name = plot.Name;
			_box = Create.ThemedGroup(app.Settings, plot, MakeContent(app));
			_box.Padding = new(Padding);
			return _box;
		}

		static UIElement MakeContent(AppModel app)
		{
			var synth = app.Track.Synth;
			var result = new DockPanel();
			result.Add(SubUI.MakeContent(app, synth.Plot), Dock.Top);
			_container = new ContentControl();
			synth.ParamChanged += BeginUpdate;
			_container.SizeChanged += BeginUpdate;
			app.Settings.PropertyChanged += BeginUpdate;
			var border = result.Add(SubUI.MakeOuterBorder(_container), Dock.Top);
			border.BorderThickness = new(SubUI.BorderThickness, 0, SubUI.BorderThickness, SubUI.BorderThickness);
			return result;
		}

		static int paint = 0;
		static void Update(RequestPlotDataEventArgs args)
		{
			int w = (int)_container.ActualWidth;
			double h = _container.ActualHeight;
			_container.Content = args.Samples.Count > 0 ? Plot(w, h, args) : Empty;
			string header = $"{_name} @ {args.SampleRate}Hz";
			header += $"{Environment.NewLine}{args.Samples.Count} samples";
			if (args.Freq != 0.0f) header += $" @ {args.Freq.ToString("N1")}Hz";
			if (args.Clip) header += " (Clip)";
			_box.Header = header + " " + paint++;
		}

		static UIElement PlotLine(int w, double h, RequestPlotDataEventArgs args)
		{
			var result = new Polyline();
			result.StrokeThickness = 1.5;
			result.Points = PlotData(w, h, args);
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

		static UIElement Plot(int w, double h, RequestPlotDataEventArgs args)
		{
			var result = new Canvas();
			result.Add(PlotLine(w, h, args));
			if (args.Bipolar)
				result.Add(Marker(0, w, h / 2.0, h / 2.0));
			for (int i = 0; i < args.Splits.Count; i++)
			{
				double pos = args.Splits[i] / (args.Samples.Count - 1.0);
				result.Add(Marker(pos * w, pos * w, 0, h));
			}
			result.VerticalAlignment = VerticalAlignment.Stretch;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		static PointCollection PlotData(int w, double h, RequestPlotDataEventArgs args)
		{
			var result = new PointCollection();
			for (int i = 0; i < args.Samples.Count; i++)
			{
				var samplePos = (double)i / args.Samples.Count;
				var xSample = samplePos * args.Samples.Count;
				var weight = xSample - (int)xSample;
				var x1 = (int)Math.Ceiling(xSample);
				var y0 = (1.0 - weight) * args.Samples[(int)xSample];
				var y1 = weight * args.Samples[x1];
				var y = (1.0f - MaxLevelUni) * h + (1.0 - (y0 + y1)) * MaxLevelUni * h;
				if (args.Bipolar) y = (-(y0 + y1) * MaxLevelBi / 2.0f + 0.5) * h;
				var screenPos = i / (args.Samples.Count - 1.0);
				result.Add(new Point(screenPos * w, y));
			}
			return result;
		}
	}
}