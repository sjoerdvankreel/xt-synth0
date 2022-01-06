﻿using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class ParamUI
	{
		static string Tooltip(SynthModel model, Param param)
		{
			var auto = model.Auto(param);
			var result = new StringBuilder();
			result.AppendLine(param.Info.Name);
			result.AppendLine($"Range: {param.Info.Min} .. {param.Info.Max}");
			result.Append("Automation target: ");
			result.AppendLine(auto?.Index.ToString("X2") ?? "none");
			result.Append("Right-click to set exact value");
			return result.ToString();
		}

		internal static void Add(
			Grid grid, AppModel model, Param param, Cell cell)
		{
			grid.Add(MakeValue(param, cell.Right(2)));
			grid.Add(Create.Label(param.Info.Name, cell.Right(1)));
			if (param.Info.IsToggle)
				grid.Add(MakeToggle(model, param, cell));
			else
				grid.Add(MakeKnob(model, param, cell));
		}

		static UIElement MakeValue(Param param, Cell cell)
		{
			var binding = Bind.Format(param);
			var result = Create.Element<Label>(cell);
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeToggle(AppModel model, Param param, Cell cell)
		{
			var result = Create.Element<CheckBox>(cell);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.ToolTip = Tooltip(model.Track.Synth, param);
			return result;
		}

		static UIElement MakeKnob(AppModel model, Param param, Cell cell)
		{
			var result = Create.Element<Knob>(cell);
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => ExactUI.Show(model.Settings, param);
			result.ToolTip = Tooltip(model.Track.Synth, param);
			return result;
		}
	}
}