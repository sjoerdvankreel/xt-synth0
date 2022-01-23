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

		static UIElement MakeRight(AppModel app)
		{
			var result = new DockPanel();
			result.Add(SubUI.Make(app, app.Track.Seq.Edit), Dock.Bottom);
			result.Add(PatternUI.Make(app), Dock.Bottom);
			return result;
		}

		static UIElement MakeLeft(AppModel app)
		{
			var synth = app.Track.Synth;
			var result = new DockPanel();
			foreach (var unit in synth.Units)
				result.Add(SubUI.Make(app, unit), Dock.Top);
			foreach (var lfo in synth.Lfos)
				result.Add(SubUI.Make(app, lfo), Dock.Top);
			return result;
		}

		static UIElement MakeCenter(AppModel app)
		{
			var synth = app.Track.Synth;
			var envCount = Model.Model.EnvCount;
			var result = Create.Grid(envCount + 4, 1);
			for (int i = 0; i < envCount; i++)
				result.Add(SubUI.Make(app, synth.Envs[i]), new(i, 0));
			result.Add(PlotUI.Make(app), new(envCount + 0, 0));
			result.Add(SubUI.Make(app, synth.Global), new(envCount + 1, 0));
			result.Add(MonitorUI.Make(app), new(envCount + 2, 0));
			result.Add(ControlUI.Make(app), new(envCount + 3, 0));
			result.RowDefinitions[envCount].Height = new GridLength(1.0, GridUnitType.Star);
			return result;
		}
	}
}