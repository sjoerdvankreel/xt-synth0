using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal static UIElement Make(
			IList<RowModel> rows, string name, Cell cell)
		{
			var result = UI.MakeElement<GroupBox>(cell);
			result.Header = name;
			result.Content = MakeContent(rows);
			return result;
		}

		static UIElement MakeContent(IList<RowModel> rows)
		{
			var result = UI.MakeGrid(rows.Count, 3);
			for (int r = 0; r < rows.Count; r++)
				RowUI.Add(result, rows[r], r);
			return result;
		}
	}
}