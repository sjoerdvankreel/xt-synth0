﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class SubUI
	{
		internal const double BorderThickness = 1;

		internal static FrameworkElement MakeContent(AppModel app, IThemedSubModel sub)
		=> MakeOuterBorder(MakeEnabled(app, sub));

		internal static GroupBox Make(AppModel app, IThemedSubModel sub)
		{
			var result = Create.ThemedGroup(app.Settings, sub, MakeContent(app, sub));
			if (sub.Enabled == null) return result;
			var wrap = new WrapPanel();
			wrap.Add(Create.Text(sub.Name));
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsStopped));
			var enabled = wrap.Add(ParamUI.MakeControl(app, sub, sub.Enabled));
			enabled.Margin = new(3.0, 0.0, 0.0, 0.0);
			enabled.SetBinding(Control.IsEnabledProperty, binding);
			result.Header = wrap;
			return result;
		}

		internal static Border MakeOuterBorder(UIElement child)
		{
			var result = new Border();
			result.Child = child;
			result.SnapsToDevicePixels = true;
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			result.BorderThickness = new(0, 0, BorderThickness, BorderThickness);
			return result;
		}

		static Border MakeInnerBorder(UIElement child, Cell cell)
		{
			var result = Create.Element<Border>(cell);
			result.SnapsToDevicePixels = true;
			result.BorderThickness = new(BorderThickness, BorderThickness, 0, 0);
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			result.Child = child;
			return result;
		}

		static UIElement MakeEnabled(AppModel app, IThemedSubModel sub)
		{
			var result = Create.Grid(1, 1);
			var grid = result.Add(MakeGrid(app, sub));
			if (sub.Enabled == null) return result;
			var conv = new VisibilityConverter<int>(true, 1);
			var binding = Bind.To(sub.Enabled, nameof(Param.Value), conv);
			grid.SetBinding(UIElement.VisibilityProperty, binding);
			var off = result.Add(Create.Label("Off"));
			conv = new VisibilityConverter<int>(true, 0);
			binding = Bind.To(sub.Enabled, nameof(Param.Value), conv);
			off.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static Grid MakeGrid(AppModel app, IThemedSubModel sub)
		{
			int cols = sub.ColumnCount;
			var positions = sub.ParamLayout.Max(p => p.Value) + 1;
			int rows = (int)Math.Ceiling(positions / (double)cols);
			var result = Create.Grid(rows, cols);
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.RowDefinitions[rows - 1].Height = new GridLength(1.0, GridUnitType.Star);
			result.ColumnDefinitions[cols - 1].Width = new GridLength(1.0, GridUnitType.Star);
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			foreach (var p in sub.ParamLayout)
				if (p.Value >= 0)
					result.Add(MakeInnerBorder(ParamUI.Make(app, sub, p.Key), new(p.Value / cols, p.Value % cols)));
			for (int p = 0; p < rows * cols; p++)
				if (!sub.ParamLayout.Values.Contains(p))
					result.Add(MakeInnerBorder(null, new Cell(p / cols, p % cols)));
			return result;
		}
	}
}