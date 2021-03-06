using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
    public static unsafe class PlotUI
    {
        const int VPadText = 7;
        const int HPadText = 3;
        const int PadLeft = 30;
        const int PadBottom = 20;
        const double MaxLevel = 0.99;

        class PlotData
        {
            internal PointCollection l;
            internal PointCollection r;
        }

        static Action _update;
        static bool _updating = false;
        public static void BeginUpdate() => _updating = true;

        static readonly FrameworkElement Off = MakeOff();
        public static event EventHandler<RequestPlotDataEventArgs> RequestPlotData;

        public static void EndUpdate()
        {
            _updating = false;
            _update();
        }

        static FrameworkElement MakeOff()
        {
            var dock = new DockPanel();
            var label = dock.Add(Create.Label("Plot OFF"));
            label.FontWeight = FontWeights.Bold;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.SetResourceReference(Control.ForegroundProperty, Utility.RowDisabledKey);
            return Create.ThemedContent(dock);
        }

        internal static GroupBox Make(AppModel app)
        {
            var text = new TextBlock();
            var plot = app.Track.Synth.Plot;
            text.VerticalAlignment = VerticalAlignment.Center;
            text.HorizontalAlignment = HorizontalAlignment.Right;
            var result = Create.ThemedGroup(app.Settings, plot, MakeContent(app, text));
            var dock = new DockPanel();
            var wrap = dock.Add(new WrapPanel(), Dock.Left);
            wrap.Add(Create.Text(plot.Name));
            var enabled = wrap.Add(ParamUI.MakeControl(app, plot.ThemeGroup, plot.Enabled));
            enabled.Margin = new(3.0, 0.0, 3.0, 0.0);
            dock.Add(text, Dock.Right);
            result.Header = dock;
            return result;
        }

        static void PlotProperties(Shape shape, Brush foreground1)
        {
            shape.Opacity = 0.67;
            shape.Stroke = foreground1;
        }

        static UIElement PlotLine(PointCollection data, Brush foreground1)
        {
            var result = new Polyline();
            result.Points = data;
            result.StrokeThickness = 1.5;
            PlotProperties(result, foreground1);
            return result;
        }

        static UIElement Marker(double x, double y, string text)
        {
            var result = Create.Text(text);
            Canvas.SetLeft(result, x);
            Canvas.SetTop(result, y);
            return result;
        }

        static UIElement Split(double x1, double x2, double y1, double y2, Brush foreground2)
        {
            var result = new Line();
            result.X1 = x1;
            result.X2 = x2;
            result.Y1 = y1;
            result.Y2 = y2;
            result.Opacity = 0.5f;
            result.Stroke = foreground2;
            result.StrokeDashArray = new DoubleCollection(new[] { 4.0, 2.0 });
            return result;
        }

        static UIElement PlotBar(Point p, double h, double stroke, double @base, Brush foreground1)
        {
            var result = new Line();
            result.X1 = p.X;
            result.X2 = p.X;
            result.Y2 = p.Y;
            result.Y1 = (h - PadBottom) * @base + VPadText;
            result.StrokeThickness = stroke;
            PlotProperties(result, foreground1);
            return result;
        }

        static PlotData MakePlotData(RequestPlotDataEventArgs args, int w, double h)
        {
            var result = new PlotData();
            double scale = args.Output->stereo != 0 ? 0.5 : 1.0;
            double baseL = args.Output->stereo != 0 ? 0.5f : 0.0f;
            double baseR = (1.0f - (args.Output->max - args.Output->min)) / 2.0f;
            result.l = MakeChannelData(args, args.Result->left, w, h, baseL, scale);
            if (args.Output->stereo != 0) result.r = MakeChannelData(args, args.Result->right, w, h, baseR, 0.5);
            return result;
        }

        static UIElement MakeContent(AppModel app, TextBlock text)
        {
            var result = new Grid();
            var dock = new DockPanel();
            var plot = app.Track.Synth.Plot;
            dock.Add(GroupUI.MakeContent(app, app.Track.Synth.Plot), Dock.Top);
            dock.Add(MakePlotContent(app, text), Dock.Top);
            var conv = new VisibilityConverter<int>(true, 1);
            var binding = Bind.To(plot.Enabled, nameof(Param.Value), conv);
            dock.SetBinding(UIElement.VisibilityProperty, binding);
            result.Add(dock);
            var off = MakeOff();
            conv = new VisibilityConverter<int>(true, 0);
            binding = Bind.To(plot.Enabled, nameof(Param.Value), conv);
            off.SetBinding(UIElement.VisibilityProperty, binding);
            result.Add(off);
            return result;
        }

        static UIElement MakePlotContent(AppModel app, TextBlock text)
        {
            var synth = app.Track.Synth;
            var dock = new DockPanel();
            var result = Create.ThemedContent(dock);
            var content = dock.Add(new ContentControl());
            _update = () => Update(app, text, content);
            synth.ParamChanged += (s, e) => _update();
            content.SizeChanged += (s, e) => _update();
            app.Settings.PropertyChanged += (s, e) => _update();
            app.Track.Seq.Edit.Bpm.PropertyChanged += (s, e) => _update();
            result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
            result.BorderThickness = new(GroupUI.BorderThickness, 0.0, GroupUI.BorderThickness, GroupUI.BorderThickness);
            return result;
        }

        static PointCollection MakeChannelData(RequestPlotDataEventArgs args, float* samples, int w, double h, double @base, double scale)
        {
            int width = w - PadLeft;
            double hPad = h - PadBottom;
            var result = new PointCollection();
            for (int i = 0; i <= width; i++)
            {
                var xScreen = (double)i / width;
                var xSample = xScreen * (args.Result->sampleCount - 1);
                var weight = xSample - (int)xSample;
                var x1 = (int)Math.Ceiling(xSample);
                var y0 = (1.0 - weight) * samples[(int)xSample];
                var y1 = weight * samples[x1];
                var y = @base + scale * (y0 + y1);
                var yScreen = (1.0f - MaxLevel) * hPad + (1.0 - y) * MaxLevel * hPad;
                var l = PadLeft + xScreen * (w - PadLeft);
                if (args.Output->max - args.Output->min == 0.0f)
                    throw new InvalidOperationException();
                Point p = new Point(l, VPadText + yScreen / (args.Output->max - args.Output->min));
                if (double.IsNaN(p.X) || double.IsNaN(p.Y))
                    throw new InvalidOperationException();
                result.Add(p);
            }
            return result;
        }

        static void Update(AppModel app, TextBlock text, ContentControl container)
        {
            if (_updating) return;
            var plot = app.Track.Synth.Plot;
            if (plot.On.Value == 0)
            {
                text.Text = null;
                container.Content = Off;
                return;
            }
            double h = container.ActualHeight;
            int w = (int)container.ActualWidth;
            var args = new RequestPlotDataEventArgs(w - PadLeft);
            text.Text = null;
            container.Content = Off;
            RequestPlotData?.Invoke(null, args);
            if (args.Result->sampleCount == 0) return;
            var resources = Utility.GetThemeResources(app.Settings, plot.ThemeGroup);
            var foreground1 = (Brush)resources[Utility.Foreground1Key];
            var foreground2 = (Brush)resources[Utility.Foreground2Key];
            container.Content = Plot(args, w, h, foreground1, foreground2);
            string header = $"{args.Result->sampleCount} samples";
            if (args.Output->frequency != 0.0f) header += $" @ {args.Output->frequency.ToString("N1")}Hz";
            if (args.Output->clip != 0) header += " (Clip)";
            text.Text = header;
        }

        static IList<UIElement> MakeHorizontalMarkers(RequestPlotDataEventArgs args, int w, double h, Brush foreground2)
        {
            var result = new List<UIElement>();
            int sampleCount = args.Result->sampleCount;
            for (int i = 0; i < args.Result->horizontalCount; i++)
            {
                double pos = args.Result->horizontalPositions[i] / (sampleCount - 1.0);
                double l = PadLeft + pos * (w - PadLeft);
                result.Add(Split(l, l, VPadText, VPadText + h - PadBottom, foreground2));
                result.Add(Marker(l - HPadText, h - PadBottom + VPadText, args.Result->HorizontalText(i)));
            }
            return result;
        }

        static IList<UIElement> MakeVerticalMarkers(RequestPlotDataEventArgs args, int w, double h, Brush foreground2)
        {
            double hPad = h - PadBottom;
            float max = args.Output->max;
            float min = args.Output->min;
            var result = new List<UIElement>();
            for (int i = 0; i < args.Result->verticalCount; i++)
            {
                double pos = (args.Result->verticalPositions[i] - min) / (max - min);
                double y = VPadText + pos * hPad;
                result.Add(Split(PadLeft, w, y, y, foreground2));
                result.Add(Marker(0, pos * hPad, args.Result->VerticalText(i).PadLeft(4)));
            }
            return result;
        }

        static IList<UIElement> MakeAudioPlot(RequestPlotDataEventArgs args, PlotData data, Brush foreground1)
        {
            var result = new List<UIElement>();
            result.Add(PlotLine(data.l, foreground1));
            if (args.Output->stereo != 0) result.Add(PlotLine(data.r, foreground1));
            return result;
        }

        static IList<UIElement> MakeSpectrumPlot(RequestPlotDataEventArgs args, PlotData data, int w, double h, Brush foreground1)
        {
            var result = new List<UIElement>();
            for (int i = 0; i < data.l.Count; i++)
            {
                double @baseL = args.Output->stereo != 0 ? 0.5 : 1.0;
                result.Add(PlotBar(data.l[i], h, (double)w / data.l.Count, @baseL, foreground1));
                if (args.Output->stereo != 0) result.Add(PlotBar(data.r[i], h, (double)w / data.r.Count, 1.0, foreground1));
            }
            return result;
        }

        static UIElement Plot(RequestPlotDataEventArgs args, int w, double h, Brush foreground1, Brush foreground2)
        {
            var result = new Canvas();
            var data = MakePlotData(args, w, h);
            result.VerticalAlignment = VerticalAlignment.Stretch;
            result.HorizontalAlignment = HorizontalAlignment.Stretch;
            result.AddRange(MakeVerticalMarkers(args, w, h, foreground2));
            result.AddRange(MakeHorizontalMarkers(args, w, h, foreground2));
            if (args.Output->spectrum == 0) result.AddRange(MakeAudioPlot(args, data, foreground1));
            else result.AddRange(MakeSpectrumPlot(args, data, w, h, foreground1));
            return result;
        }
    }
}