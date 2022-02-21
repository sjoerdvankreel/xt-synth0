using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
    public static class PlotUI
    {
        const int VPadText = 7;
        const int HPadText = 3;
        const int PadLeft = 18;
        const int PadBottom = 20;
        const double MaxLevel = 0.99;

        class PlotData
        {
            internal PointCollection l;
            internal PointCollection r;
        }

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

        static PointCollection MakeChannelData(List<float> samples,
            int w, double h, float min, float max, double @base, double scale)
        {
            double hPad = h - PadBottom;
            var result = new PointCollection();
            for (int i = 0; i < samples.Count; i++)
            {
                var samplePos = (double)i / samples.Count;
                var xSample = samplePos * samples.Count;
                var weight = xSample - (int)xSample;
                var x1 = (int)Math.Ceiling(xSample);
                var y0 = (1.0 - weight) * samples[(int)xSample];
                var y1 = weight * samples[x1];
                var y = @base + scale * (y0 + y1);
                var yScreen = (1.0f - MaxLevel) * hPad + (1.0 - y) * MaxLevel * hPad;
                var screenPos = i / (samples.Count - 1.0);
                var l = PadLeft + screenPos * (w - PadLeft);
                if (max - min == 0.0f)
                    throw new InvalidOperationException();
                Point p = new Point(l, VPadText + yScreen / (max - min));
                if (double.IsNaN(p.X) || double.IsNaN(p.Y))
                    throw new InvalidOperationException();
                result.Add(p);
            }
            return result;
        }

        static PlotData MakePlotData(int w, double h, float min, float max)
        {
            var result = new PlotData();
            double scale = Args.Stereo ? 0.5 : 1.0;
            double baseL = Args.Stereo ? 0.5f : 0.0f;
            double baseR = (1.0f - (max - min)) / 2.0f;
            result.l = MakeChannelData(Args.LSamples, w, h, min, max, baseL, scale);
            if (Args.Stereo) result.r = MakeChannelData(Args.RSamples, w, h, min, max, baseR, 0.5);
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

        static UIElement PlotBar(Point p, double h, double stroke, double @base)
        {
            var result = new Line();
            result.X1 = p.X;
            result.X2 = p.X;
            result.Y2 = p.Y;
            result.Y1 = (h - PadBottom) * @base + VPadText;
            result.StrokeThickness = stroke;
            PlotProperties(result);
            return result;
        }

        static void Update(AppModel app, TextBlock text, ContentControl container)
        {
            var plot = app.Track.Synth.Plot;
            int w = (int)container.ActualWidth;
            double h = container.ActualHeight;
            Args.Pixels = w;
            RequestPlotData?.Invoke(null, Args);
            container.Content = Args.LSamples.Count > 0 ? Plot(w, h, Args.Min, Args.Max) : Off;
            string header = $"{plot.Name}";
            header += Args.Freq == 0.0f ? " @ " : " ";
            header += $"{Args.LSamples.Count} samples";
            if (Args.Freq != 0.0f) header += $" @ {Args.Freq.ToString("N1")}Hz";
            if (Args.Clip) header += " (Clip)";
            text.Text = header;
        }

        static UIElement Plot(int w, double h, float min, float max)
        {
            var result = new Canvas();
            double hPad = h - PadBottom;
            var data = MakePlotData(w, h, min, max);
            result.VerticalAlignment = VerticalAlignment.Stretch;
            result.HorizontalAlignment = HorizontalAlignment.Stretch;

            for (int i = 0; i < Args.VSplitVals.Count; i++)
            {
                double pos = (Args.VSplitVals[i] - min) / (max - min);
                double y = VPadText + pos * hPad;
                result.Add(Split(PadLeft, w, y, y));
                result.Add(Marker(0, pos * hPad, Args.VSplitMarkers[i].PadLeft(2)));
            }

            for (int i = 0; i < Args.HSplitVals.Count; i++)
            {
                double pos = Args.HSplitVals[i] / (Args.LSamples.Count - 1.0);
                double l = PadLeft + pos * (w - PadLeft);
                result.Add(Split(l, l, VPadText, VPadText + h - PadBottom));
                result.Add(Marker(l - HPadText, h - PadBottom + VPadText, Args.HSplitMarkers[i]));
            }

            if (!Args.Spectrum)
            {
                result.Add(PlotLine(data.l));
                if (Args.Stereo) result.Add(PlotLine(data.r));
            }
            else
                for (int i = 0; i < data.l.Count; i++)
                {
                    double @baseL = Args.Stereo ? 0.5 : 1.0;
                    result.Add(PlotBar(data.l[i], h, (double)w / data.l.Count, @baseL));
                    if (Args.Stereo) result.Add(PlotBar(data.r[i], h, (double)w / data.r.Count, 1.0));
                }
            return result;
        }
    }
}