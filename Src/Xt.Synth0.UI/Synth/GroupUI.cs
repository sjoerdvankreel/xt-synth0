using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class GroupUI
	{
		static UIElement MakeContent(IGroupModel model)
		{
			var rows = model.Params();
			var columns = rows.Max(r => r.Length);
			var result = UI.MakeGrid(rows.Length, columns * 3);
			for (int r = 0; r < rows.Length; r++)
				for (int c = 0; c < rows[r].Length; c++)
					if (model.Params()[r][c].Info.Type == ParamType.Toggle)
						ToggleUI.Add(result, rows[r][c], r, c * 3);
					else
						KnobUI.Add(result, rows[r][c], r, c * 3);
			return result;
		}

		internal static UIElement Make(IGroupModel model, string name,
			int row, int column, int rowSpan = 1, int columnSpan = 1)
		{
			var result = UI.MakeElement<GroupBox>(row, column, rowSpan, columnSpan);
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}
	}
}