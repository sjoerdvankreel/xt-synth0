using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
    public static class TrackUI
    {
        public static UIElement Make(AppModel app)
        {
            var result = new DockPanel();
            result.Add(MakeLeft(app), Dock.Left);
            result.Add(MakeCenter(app), Dock.Left);
            result.Add(MakeRight(app), Dock.Left);
            return result;
        }

        static UIElement MakeLeft(AppModel app)
        {
            var synth = app.Track.Synth;
            var result = new DockPanel();
            foreach (var unit in synth.Units)
                result.Add(GroupUI.Make(app, unit), Dock.Top);
            foreach (var filter in synth.Filters)
                result.Add(GroupUI.Make(app, filter), Dock.Top);
            return result;
        }

        static UIElement MakeRight(AppModel app)
        {
            var result = new DockPanel();
            result.Add(ControlUI.Make(app), Dock.Bottom);
            result.Add(MonitorUI.Make(app), Dock.Bottom);
            var edit = result.Add(GroupUI.Make(app, app.Track.Seq.Edit), Dock.Bottom);
            var binding = Bind.To(app.Stream, nameof(app.Stream.IsStopped));
            edit.SetBinding(UIElement.IsEnabledProperty, binding);
            result.Add(PatternUI.Make(app), Dock.Top);
            return result;
        }

        static UIElement MakeCenter(AppModel app)
        {
            var synth = app.Track.Synth;
            var envCount = Model.Model.EnvCount;
            var lfoCount = Model.Model.LfoCount;
            var result = Create.Grid(envCount + lfoCount + 2, 1);
            for (int i = 0; i < envCount; i++)
                result.Add(GroupUI.Make(app, synth.Envs[i]), new(i, 0));
            for (int i = 0; i < lfoCount; i++)
                result.Add(GroupUI.Make(app, synth.Lfos[i]), new(envCount + i, 0));
            result.Add(GroupUI.Make(app, app.Track.Synth.Global), new(envCount + lfoCount + 0, 0));
            result.Add(PlotUI.Make(app), new(envCount + lfoCount + 1, 0));
            result.RowDefinitions[envCount + lfoCount + 1].Height = new GridLength(1.0, GridUnitType.Star);
            return result;
        }
    }
}