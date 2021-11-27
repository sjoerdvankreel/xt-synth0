using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal static UIElement Make(
			PatternModel model, string name)
		{
			var result = new GroupBox();
			result.Header = name;
			result.Content = MakeContent(model);
			return result;
		}

		static UIElement MakeContent(PatternModel model)
		{
			var rowCount = PatternModel.RowCount;
			var result = UI.MakeGrid(rowCount, 20);
			for (int r = 0; r < rowCount; r++)
			{
				PatternKeyUI.Add(result, model.Rows[r].Key1, r, 0);
				result.Children.Add(UI.MakeDivider(new(r, 4)));
				PatternKeyUI.Add(result, model.Rows[r].Key2, r, 5);
				result.Children.Add(UI.MakeDivider(new(r, 9)));
				PatternKeyUI.Add(result, model.Rows[r].Key3, r, 10);
				result.Children.Add(UI.MakeDivider(new(r, 14)));
				PatternFxUI.Add(result, model.Rows[r].Fx1, r, 15);
				result.Children.Add(UI.MakeDivider(new(r, 17)));
				PatternFxUI.Add(result, model.Rows[r].Fx2, r, 18);
			}
			return result;
		}
	}
}