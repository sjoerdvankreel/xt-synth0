using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		internal static UIElement Make(SynthModel synth,
			GroupModel group, SettingsModel settings, AudioModel audio)
		{
			var result = new GroupBox();
			result.Header = group.Name();
			result.Content = MakeContent(synth, group, settings, audio);
			return result;
		}

		static UIElement MakeContent(SynthModel synth,
			GroupModel group, SettingsModel settings, AudioModel audio)
		{
			var rows = group.ParamGroups();
			var cols = rows.Max(r => r.Length);
			var result = Create.Grid(rows.Length, cols * 3);
			result.VerticalAlignment = VerticalAlignment.Top;
			for (int r = 0; r < rows.Length; r++)
				for (int c = 0; c < rows[r].Length; c++)
					ParamUI.Add(result, synth, settings, rows[r][c], new(r, c * 3));
			if (group.Automation()) return result;
			var binding = Bind.To(audio, nameof(AudioModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}
	}
}