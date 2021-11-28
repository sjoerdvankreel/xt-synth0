using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		internal static UIElement Make(
			SynthModel synth, GroupModel group)
		{
			var result = new GroupBox();
			result.Header = group.Name();
			result.Content = MakeContent(synth, group);
			return result;
		}

		static UIElement MakeContent(
			SynthModel synth, GroupModel group)
		{
			var rows = group.ParamGroups();
			var cols = rows.Max(r => r.Length);
			var result = UI.MakeGrid(rows.Length, cols * 3);
			result.VerticalAlignment = VerticalAlignment.Top;
			for (int r = 0; r < rows.Length; r++)
				for (int c = 0; c < rows[r].Length; c++)
					ParamUI.Add(result, synth, rows[r][c], new(r, c * 3));
			return result;
		}
	}
}