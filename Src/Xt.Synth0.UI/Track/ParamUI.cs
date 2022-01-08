﻿using System;
using System.Linq;
using System.Text;
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
			if (param.Info.Control == ParamControl.Knob)
				result.AppendLine($"Range: {param.Info.Min} .. {param.Info.Max}");
			if (auto != null)
				result.AppendLine($"Automation target: {auto?.Index.ToString("X2")}");
			if (param.Info.Control == ParamControl.Knob)
				result.AppendLine("Right-click to set exact value");
			return result.ToString(0, result.Length - Environment.NewLine.Length);
		}

		internal static DockPanel MakeEmpty(Cell cell)
		{
			var result = Create.Element<DockPanel>(cell);
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		internal static UIElement Make(
			AppModel app, ISubModel sub, Param param, Cell cell)
		{
			var result = MakeEmpty(cell);
			bool conditional = param.Info.Relevant(sub) != null;
			if (conditional) result.SetBinding(UIElement.VisibilityProperty, Bind.Relevant(sub, param));
			var control = result.Add(MakeControl(app, param), Dock.Left);
			if (param.Info.Type == ParamType.List) return result;
			var label = result.Add(Create.Label(param.Info.Name), Dock.Left);
			//if (conditional) label.SetBinding(UIElement.VisibilityProperty, Bind.Relevant(sub, param));
			if (param.Info.Type == ParamType.Toggle) return result;
			var value = result.Add(MakeValue(param), Dock.Left);
			//if (conditional) value.SetBinding(UIElement.VisibilityProperty, Bind.Relevant(sub, param));
			return result;
		}

		static Control MakeControl(AppModel model, Param param)
		=> param.Info.Type switch
		{
			ParamType.Lin => MakeKnob(model, param),
			ParamType.Exp => MakeKnob(model, param),
			ParamType.Quad => MakeKnob(model, param),
			ParamType.List => MakeList(model, param),
			ParamType.Toggle => MakeToggle(model, param),
			_ => throw new InvalidOperationException()
		};

		static Control MakeValue(Param param)
		{
			var result = new Label();
			var binding = Bind.Format(param);
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static Control MakeToggle(AppModel model, Param param)
		{
			var result = new CheckBox();
			result.ToolTip = Tooltip(model.Track.Synth, param);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			return result;
		}

		static Control MakeKnob(AppModel model, Param param)
		{
			var result = new Knob();
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.ToolTip = Tooltip(model.Track.Synth, param);
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => ExactUI.Show(model.Settings, param);
			return result;
		}

		static Control MakeList(AppModel model, Param param)
		{
			var result = new ComboBox();
			result.SelectedValuePath = nameof(ListItem.Value);
			result.ToolTip = Tooltip(model.Track.Synth, param);
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			var range = Enumerable.Range(param.Info.Min, param.Info.Max - param.Info.Min + 1);
			var items = range.Select(i => new ListItem(param.Info, i)).ToArray();
			result.SetBinding(Selector.SelectedValueProperty, Bind.To(param));
			result.SetValue(ItemsControl.ItemsSourceProperty, items);
			return result;
		}
	}
}