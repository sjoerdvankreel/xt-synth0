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
			result.VerticalAlignment = VerticalAlignment.Top;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.ColumnDefinitions[cols - 1].Width = new GridLength(1.0, GridUnitType.Star);
			for (int p = 0; p < @params.Count; p++)
			{
				int r = p / cols;
				int c = p % cols;
				result.Add(ParamUI.Make(model, @params[p], new(r, c)));
			}
			return result;
		}
	}
}