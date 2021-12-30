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
			var result = Create.Grid(rows, cols * 3);
			result.VerticalAlignment = VerticalAlignment.Top;
			for (int p = 0; p < @params.Count; p++)
			{
				int r = p / cols;
				int c = p % cols;
				ParamUI.Add(result, model, @params[r * cols + c], new(r, c * 3));
			}
			return result;
		}
	}
}