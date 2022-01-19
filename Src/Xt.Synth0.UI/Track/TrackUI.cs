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
			result.Add(PatternUI.Make(app), Dock.Left);
			return result;
		}

		static UIElement MakeLeft(AppModel app)
		{
			var synth = app.Track.Synth;
			var result = new DockPanel();
			foreach (var unit in synth.Units)
				result.Add(SubUI.Make(app, unit), Dock.Top);
			result.Add(SubUI.Make(app, synth.Global), Dock.Top);
			result.Add(ControlUI.Make(app), Dock.Top);
			return result;
		}

		static UIElement MakeCenter(AppModel app)
		{
			var synth = app.Track.Synth;
			var envCount = Model.Model.EnvCount;
			var result = Create.Grid(envCount + 2, 1);
			for (int i = 0; i < envCount; i++)
				result.Add(SubUI.Make(app, synth.Envs[i]), new(i, 0));
			result.Add(PlotUI.Make(app), new(envCount + 0, 0));
			result.Add(EditUI.Make(app), new(envCount + 1, 0));
			result.Add(MonitorUI.Make(app), new(envCount + 1, 0));
			result.RowDefinitions[envCount].Height = new GridLength(1.0, GridUnitType.Star);
			return result;
		}
	}
}