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
			var result = Create.PatternCell<HexBox>(new(row, col));
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.SetBinding(UIElement.VisibilityProperty, Bind.Show(fxCount, minFx));
			result.OnParsed += (s, e) => Utility.FocusNext(FocusNavigationDirection.Next);
			result.ToolTip = string.Join("\n", param.Info.Detail, PatternUI.EditHint);
			return result;
		}
	}
}