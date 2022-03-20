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
            result.Add(MakeSynth(app), Dock.Left);
            result.Add(MakeSeq(app), Dock.Left);
            return result;
        }

        static UIElement MakeSynth(AppModel app)
        {
            var result = new DockPanel();
            result.Add(MakeSynthLeft(app), Dock.Left);
            result.Add(MakeSynthRight(app), Dock.Left);
            return result;
        }

        static UIElement MakeSynthLeft(AppModel app)
        {
            var synth = app.Track.Synth;
            var result = new DockPanel();
            for (int i = 0; i < SynthConfig.UnitCount; i++)
                result.Add(GroupUI.Make(app, synth.Units[i]), Dock.Top);
            for (int i = 0; i < SynthConfig.LfoCount; i++)
                result.Add(GroupUI.Make(app, synth.Lfos[i]), Dock.Top);
            result.Add(PlotUI.Make(app), Dock.Top);
            return result;
        }

        static UIElement MakeSynthRight(AppModel app)
        {
            var synth = app.Track.Synth;
            var result = new DockPanel();
            for (int i = 0; i < SynthConfig.EnvCount; i++)
                result.Add(GroupUI.Make(app, synth.Envs[i]), Dock.Top);
            for (int i = 0; i < SynthConfig.FilterCount; i++)
                result.Add(GroupUI.Make(app, synth.Filters[i]), Dock.Top);
            result.Add(GroupUI.Make(app, synth.Amp), Dock.Top);
            return result;
        }

        static UIElement MakeSeq(AppModel app)
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
    }
}