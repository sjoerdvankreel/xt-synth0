using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GroupUI
	{
		internal const double BorderThickness = 1;

		internal static FrameworkElement MakeContent(AppModel app, IUIParamGroupModel group)
		=> MakeOuterBorder(group.Enabled == null ? MakeGrid(app, group) : MakeEnabled(app, group));

		internal static GroupBox Make(AppModel app, IUIParamGroupModel group)
		{
			var result = Create.ThemedGroup(app.Settings, group, MakeContent(app, group));
			if (group.Enabled == null) return result;
			var wrap = new WrapPanel();
			wrap.Add(Create.Text(group.Name));
			var enabled = wrap.Add(ParamUI.MakeControl(app, group.ThemeGroup, group.Enabled));
			enabled.Margin = new(3.0, 0.0, 0.0, 0.0);
			result.Header = wrap;
			return result;
		}

		static Border MakeOuterBorder(UIElement child)
		{
			var result = new Border();
			result.Child = child;
			result.SnapsToDevicePixels = true;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.BorderThickness = new(0, 0, BorderThickness, BorderThickness);
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			return Create.ThemedContent(result);
		}

		static Border MakeInnerBorder(UIElement child, Cell cell)
		{
			var result = Create.Element<Border>(cell);
			result.Padding = new(0.0);
			result.SnapsToDevicePixels = true;
			result.BorderThickness = new(BorderThickness, BorderThickness, 0, 0);
			result.SetResourceReference(Border.BorderBrushProperty, Utility.BorderParamKey);
			result.Child = child;
			return result;
		}

		static UIElement MakeEnabled(AppModel app, IUIParamGroupModel group)
		{
			var result = Create.Grid(1, 1);
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
			var grid = result.Add(MakeGrid(app, group));
			var conv = new VisibilityConverter<int>(true, 1);
			var binding = Bind.To(group.Enabled, nameof(Param.Value), conv);
			grid.SetBinding(UIElement.VisibilityProperty, binding);
			var empty = ParamUI.MakeEmpty();
			var off = result.Add(MakeInnerBorder(empty, new(0, 0)));
			conv = new VisibilityConverter<int>(true, 0);
			binding = Bind.To(group.Enabled, nameof(Param.Value), conv);
			off.SetBinding(UIElement.VisibilityProperty, binding);
			var label = empty.Add(Create.Label($"{group.Name} OFF"));
			label.FontWeight = FontWeights.Bold;
			label.VerticalAlignment = VerticalAlignment.Center;
			label.HorizontalAlignment = HorizontalAlignment.Center;
			label.SetResourceReference(Control.ForegroundProperty, Utility.RowDisabledKey);
			return result;
		}

		static Grid MakeGrid(AppModel app, IUIParamGroupModel group)
		{
			int cols = group.Columns;
			var positions = group.Layout.Max(p => p.Value) + 1;
			int rows = (int)Math.Ceiling(positions / (double)cols);
			var result = Create.Grid(rows, cols);
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.RowDefinitions[rows - 1].Height = new GridLength(1.0, GridUnitType.Star);
			result.ColumnDefinitions[cols - 1].Width = new GridLength(1.0, GridUnitType.Star);
			foreach (var p in group.Layout)
				if (p.Value >= 0)
					result.Add(MakeInnerBorder(ParamUI.Make(app, group, p.Key), new(p.Value / cols, p.Value % cols)));
			for (int p = 0; p < rows * cols; p++)
				if (!group.Layout.Values.Contains(p))
					result.Add(MakeInnerBorder(null, new Cell(p / cols, p % cols)));
			return result;
		}
	}
}