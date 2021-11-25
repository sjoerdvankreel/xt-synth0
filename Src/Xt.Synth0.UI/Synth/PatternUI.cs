using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal static UIElement Make(
			RowModel[] rows, string name, Cell cell)
		{
			var result = UI.MakeElement<GroupBox>(cell);
			result.Header = name;
			result.Content = MakeContent(rows);
			return result;
		}

		static UIElement MakeContent(RowModel[] rows)
		{
			var result = UI.MakeGrid(PatternModel.Length, 4);
			for (int r = 0; r < rows.Length; r++)
				RowUI.Add(result, rows[r], r);
			return result;
		}
	}
}