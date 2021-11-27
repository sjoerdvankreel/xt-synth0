using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class PatternFxUI
	{
		internal static void Add(
			Grid grid, PatternFx model, int row, int col)
		{
			grid.Children.Add(MakeHex(model.Target, row, col));
			grid.Children.Add(MakeHex(model.Value, row, col + 1));
		}

		static UIElement MakeHex(Param param, int row, int col)
		{
			var result = UI.MakePatternCell<Hex>(new(row, col));
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, UI.Bind(param));
			result.OnParsed += (s, e) => UI.FocusNext(FocusNavigationDirection.Next);
			return result;
		}
	}
}