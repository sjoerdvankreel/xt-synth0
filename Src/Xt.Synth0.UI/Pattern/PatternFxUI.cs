using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternFxUI
	{
		internal static void Add(Grid grid, PatternFx fx,
			Param fxCount, int minFx, int row, int col)
		{
			grid.Children.Add(MakeHex(fx.Target, fxCount, minFx, row, col));
			grid.Children.Add(MakeHex(fx.Value, fxCount, minFx, row, col + 1));
		}

		static UIElement MakeHex(Param param,
			Param fxCount, int minFx, int row, int col)
		{
			var result = UI.MakePatternCell<HexBox>(new(row, col));
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.ToolTip = $"{param.Info.Detail} {PatternUI.EditHint}";
			result.SetBinding(RangeBase.ValueProperty, UI.Bind(param));
			result.SetBinding(UIElement.VisibilityProperty, UI.Show(fxCount, minFx));
			result.OnParsed += (s, e) => UI.FocusNext(FocusNavigationDirection.Next);
			return result;
		}
	}
}