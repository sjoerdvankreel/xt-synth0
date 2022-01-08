using System;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		internal static GroupBox Make(AppModel model, INamedModel group)
		=> Create.Group(group.Name, MakeContent(model, group));

		internal static FrameworkElement MakeContent(AppModel model, INamedModel group)
		{
			const int cols = 2;
			var @params = group.Params;
			int rows = (int)Math.Ceiling(@params.Count / (double)cols);
			var result = Create.Grid(rows, cols);
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BorderParamKey);
			result.RowDefinitions[rows - 1].Height = new GridLength(1.0, GridUnitType.Star);
			result.ColumnDefinitions[cols - 1].Width = new GridLength(1.0, GridUnitType.Star);
			for (int p = 0; p < @params.Count; p++)
			{
				int r = p / cols;
				int c = p % cols;
				result.Add(ParamUI.Make(model, group, @params[p], new(r, c)));
			}
			if (@params.Count % 2 == 1)
				result.Add(ParamUI.MakeEmpty(new Cell(rows - 1, cols - 1)));
			return result;
		}
	}
}