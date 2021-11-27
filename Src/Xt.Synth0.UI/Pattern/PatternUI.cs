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
			PatternModel model, EditorModel editor)
		{
			var result = new ContentControl();
			var patterns = new UIElement[PatternModel.PatternCount];
			for (int p = 0; p < patterns.Length; p++)
				patterns[p] = MakePattern(model, editor, p);
			var binding = UI.Bind(editor.Edit);
			binding.Converter = new PatternSelector(patterns);
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakePattern(
			PatternModel model, EditorModel editor, int pattern)
		{
			var rows = PatternModel.PatternRows;
			var offset = pattern * PatternModel.PatternRows;
			var result = UI.MakeGrid(rows, 20);
			for (int r = 0; r < rows; r++)
			{
				PatternKeyUI.Add(result, model.Rows[offset + r].Key1, editor, 1, r, 0);
				result.Children.Add(UI.MakeDivider(new(r, 4), editor.Keys, 2));
				PatternKeyUI.Add(result, model.Rows[offset + r].Key2, editor, 2, r, 5);
				result.Children.Add(UI.MakeDivider(new(r, 9), editor.Keys, 3));
				PatternKeyUI.Add(result, model.Rows[offset + r].Key3, editor, 3, r, 10);
				result.Children.Add(UI.MakeDivider(new(r, 14), editor.Fx, 1));
				PatternFxUI.Add(result, model.Rows[offset + r].Fx1, editor.Fx, 1, r, 15);
				result.Children.Add(UI.MakeDivider(new(r, 17), editor.Fx, 2));
				PatternFxUI.Add(result, model.Rows[offset + r].Fx2, editor.Fx, 2, r, 18);
			}
			return result;
		}
	}
}