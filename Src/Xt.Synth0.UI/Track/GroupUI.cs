﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		const double BorderThickness = 1;

		internal static GroupBox Make(AppModel model, INamedModel group)
		=> Create.Group(group.Name, MakeContent(model, group));

		static Param[][] Layout(INamedModel group) =>
			group.ParamLayout
			.GroupBy(e => e.Value)
			.OrderBy(e => e.Key)
			.Select(e => e.Select(p => p.Key).ToArray())
			.ToArray();

		internal static FrameworkElement MakeContent(AppModel app, INamedModel group)
		{
			var border = new Border();
			border.BorderThickness = new(0, 0, BorderThickness, BorderThickness);
			border.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			border.Child = MakeGrid(app, group);
			return border;
		}

		static Border MakeBorder(UIElement child, Cell cell)
		{
			var border = Create.Element<Border>(cell);
			border.BorderThickness = new(BorderThickness, BorderThickness, 0, 0);
			border.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			border.Child = child;
			return border;
		}

		static void AddParams(Grid grid, AppModel model, 
			INamedModel group, Param[] @params, int l, int cols)
		{
			int r = l / cols;
			int c = l % cols;
			for (int p = 0; p < @params.Length; p++)
				grid.Add(MakeBorder(ParamUI.Make(model, group, @params[p]), new(r, c)));
		}

		static Grid MakeGrid(AppModel app, INamedModel group)
		{
			const int cols = 2;
			var layout = Layout(group);
			int rows = (int)Math.Ceiling(layout.Length / (double)cols);
			var result = Create.Grid(rows, cols);
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.RowDefinitions[rows - 1].Height = new GridLength(1.0, GridUnitType.Star);
			result.ColumnDefinitions[cols - 1].Width = new GridLength(1.0, GridUnitType.Star);
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			for (int l = 0; l < layout.Length; l++)
				AddParams(result, app, group, layout[l], l, cols);
			if (layout.Length % 2 == 1)
				result.Add(MakeBorder(null, new Cell(rows - 1, cols - 1)));
			return result;
		}
	}
}