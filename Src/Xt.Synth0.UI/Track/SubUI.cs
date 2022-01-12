using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class SubUI
	{
		const double BorderThickness = 1;

		internal static GroupBox Make(AppModel app, IThemedSubModel sub)
		=> Create.ThemedGroup(app.Settings, sub, MakeContent(app, sub));

		internal static FrameworkElement MakeContent(AppModel app, IThemedSubModel sub)
		{
			var result = new Border();
			result.SnapsToDevicePixels = true;
			result.BorderThickness = new(0, 0, BorderThickness, BorderThickness);
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			result.Child = MakeGrid(app, sub);
			return result;
		}

		static Border MakeBorder(UIElement child, Cell cell)
		{
			var result = Create.Element<Border>(cell);
			result.SnapsToDevicePixels = true;
			result.BorderThickness = new(BorderThickness, BorderThickness, 0, 0);
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			result.Child = child;
			return result;
		}

		static Grid MakeGrid(AppModel app, IThemedSubModel sub)
		{
			const int cols = 2;
			var positions = sub.ParamLayout.Max(p => p.Value) + 1;
			int rows = (int)Math.Ceiling(positions / (double)cols);
			var result = Create.Grid(rows, cols);
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.RowDefinitions[rows - 1].Height = new GridLength(1.0, GridUnitType.Star);
			result.ColumnDefinitions[cols - 1].Width = new GridLength(1.0, GridUnitType.Star);
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			foreach (var p in sub.ParamLayout)
				result.Add(MakeBorder(ParamUI.Make(app, sub, p.Key), new(p.Value / cols, p.Value % cols)));
			if (positions % 2 == 1)
				result.Add(MakeBorder(null, new Cell(rows - 1, cols - 1)));
			return result;
		}
	}
}