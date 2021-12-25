using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		internal static GroupBox Make(AppModel model, GroupModel group)
		{
			var result = new GroupBox();
			result.Header = group.Name();
			result.Content = MakeContent(model, group);
			return result;
		}

		static UIElement MakeContent(AppModel model, GroupModel group)
		{
			var rows = group.ParamGroups();
			var cols = rows.Max(r => r.Length);
			var result = Create.Grid(rows.Length, cols * 3);
			result.VerticalAlignment = VerticalAlignment.Top;
			for (int r = 0; r < rows.Length; r++)
				for (int c = 0; c < rows[r].Length; c++)
				{
					var valueSpan = c == rows[r].Length - 1 ? 1 + 3 * (cols - c - 1) : 1;
					ParamUI.Add(result, model, rows[r][c], new(r, c * 3), valueSpan);
				}
			if (group.Automation()) return result;
			var binding = Bind.To(model.Audio, nameof(AudioModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}
	}
}