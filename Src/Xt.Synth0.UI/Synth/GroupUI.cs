using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class GroupUI
	{
		static UIElement MakeContent(IGroupModel model)
		{
			var @params = model.Params();
			var result = UI.MakeGrid(model.Params().Length, 3);
			for (int r = 0; r < @params.Length; r++)
				if (@params[r].Info.Type == ParamType.Toggle)
					ToggleUI.Add(result, @params[r], r);
				else
					KnobUI.Add(result, @params[r], r);
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