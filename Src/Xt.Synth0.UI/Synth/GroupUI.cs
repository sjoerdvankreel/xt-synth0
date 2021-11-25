using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		internal static UIElement Make(
			IGroupModel model, string name, Cell cell)
		{
			var result = UI.MakeElement<GroupBox>(cell);
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}

		static UIElement MakeContent(IGroupModel model)
		{
			var rows = model.Params();
			var cols = rows.Max(r => r.Length);
			var result = UI.MakeGrid(rows.Length, cols * 3);
			for (int r = 0; r < rows.Length; r++)
				for (int c = 0; c < rows[r].Length; c++)
					ParamUI.Add(result, rows[r][c], new(r, c * 3));
			return result;
		}
	}
}