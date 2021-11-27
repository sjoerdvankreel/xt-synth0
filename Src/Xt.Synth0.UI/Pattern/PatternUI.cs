using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternUI
	{
		internal static UIElement Make(
			PatternModel pattern, EditorModel editor, string name)
		{
			var result = new GroupBox();
			result.Margin = new Thickness(UI.Margin);
			result.Header = MakeHeader(name, editor);
			result.Content = MakeContent(pattern, editor);
			return result;
		}

		static UIElement MakeHeader(string name, EditorModel model)
		{
			var result = new WrapPanel();
			result.Children.Add(UI.MakeText(name + " "));
			result.Children.Add(UI.MakeText(UI.Bind(model.Edit)));
			result.Children.Add(UI.MakeText("/"));
			result.Children.Add(UI.MakeText(UI.Bind(model.Pats)));
			return result;
		}

		static UIElement MakeContent(
			PatternModel pattern, EditorModel editor)
		{
			var rowCount = PatternModel.RowCount;
			var result = UI.MakeGrid(rowCount, 20);
			for (int r = 0; r < rowCount; r++)
			{
				PatternKeyUI.Add(result, pattern.Rows[r].Key1, editor, 1, r, 0);
				result.Children.Add(UI.MakeDivider(new(r, 4), editor.Keys, 2));
				PatternKeyUI.Add(result, pattern.Rows[r].Key2, editor, 2, r, 5);
				result.Children.Add(UI.MakeDivider(new(r, 9), editor.Keys, 3));
				PatternKeyUI.Add(result, pattern.Rows[r].Key3, editor, 3, r, 10);
				result.Children.Add(UI.MakeDivider(new(r, 14), editor.Fx, 1));
				PatternFxUI.Add(result, pattern.Rows[r].Fx1, editor.Fx, 1, r, 15);
				result.Children.Add(UI.MakeDivider(new(r, 17), editor.Fx, 2));
				PatternFxUI.Add(result, pattern.Rows[r].Fx2, editor.Fx, 2, r, 18);
			}
			return result;
		}
	}
}