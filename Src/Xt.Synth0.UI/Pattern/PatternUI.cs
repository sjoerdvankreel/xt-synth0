using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal static UIElement Make(
			PatternModel model, string name, Cell cell)
		{
			var result = UI.MakeElement<GroupBox>(cell);
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}

		static UIElement MakeContent(PatternModel model)
		{
			var rowCount = PatternModel.RowCount;
			var result = UI.MakeGrid(rowCount, 13);
			for (int r = 0; r < rowCount; r++)
			{
				PatternKeyUI.Add(result, model.Rows[r].Key1, r, 0);
				PatternKeyUI.Add(result, model.Rows[r].Key2, r, 3);
				PatternKeyUI.Add(result, model.Rows[r].Key3, r, 6);
				PatternFxUI.Add(result, model.Rows[r].Fx1, r, 9);
				PatternFxUI.Add(result, model.Rows[r].Fx2, r, 11);
			}
			return result;
		}
	}
}