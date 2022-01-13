using System;
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
		static string Tooltip(SynthModel synth, Param param)
		{
			var result = new StringBuilder();
			result.AppendLine(param.Info.Name);
			if (param.Info.Control == ParamControl.Knob)
				result.AppendLine($"Range: {param.Info.Range}");
			if (param.Info.Automatable)
				result.AppendLine($"Automation target: {synth.AutoParam(param).Index.ToString("X2")}");
			else
				result.AppendLine("Not automatable");
			if (param.Info.Control == ParamControl.Knob)
				result.AppendLine("Right-click to set exact value");
			return result.ToString(0, result.Length - Environment.NewLine.Length);
		}

		internal static DockPanel MakeEmpty()
		{
			var result = new DockPanel();
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		internal static UIElement Make(
			AppModel app, IThemedSubModel sub, Param param)
		{
			var result = MakeEmpty();
			bool conditional = param.Info.Relevance != null;
			if (conditional) result.SetBinding(UIElement.VisibilityProperty, Bind.Relevance(sub, param));
			result.Add(MakeAutoControl(app, sub, param), Dock.Left);
			if (param.Info.Type == ParamType.List) return result;
			var name = result.Add(Create.Text($"{param.Info.Name} "), Dock.Left);
			name.VerticalAlignment = VerticalAlignment.Center;
			if (param.Info.Type == ParamType.Toggle) return result;
			result.Add(MakeValue(param), Dock.Left);
			return result;
		}

		static Control MakeAutoControl(AppModel app, IThemedSubModel sub, Param param)
		{
			var result = MakeControl(app, sub, param);
			if (param.Info.Automatable) return result; 
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static Control MakeControl(AppModel app, IThemedSubModel sub, Param param)
		=> param.Info.Type switch
		{
			ParamType.List => MakeList(app, param),
			ParamType.Toggle => MakeToggle(app, param),
			ParamType.Lin => MakeKnob(app, sub, param),
			ParamType.Exp => MakeKnob(app, sub, param),
			ParamType.Time => MakeKnob(app, sub, param),
			_ => throw new InvalidOperationException()
		};

		static TextBlock MakeValue(Param param)
		{
			var result = new TextBlock();
			var binding = Bind.Format(param);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.VerticalAlignment = VerticalAlignment.Center;
			return result;
		}

		static Control MakeToggle(AppModel app, Param param)
		{
			var result = new CheckBox();
			result.ToolTip = Tooltip(app.Track.Synth, param);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			return result;
		}

		static Control MakeList(AppModel app, Param param)
		{
			var result = new ComboBox();
			result.SelectedValuePath = nameof(ListItem.Value);
			result.ToolTip = Tooltip(app.Track.Synth, param);
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			var range = Enumerable.Range(param.Info.Min, param.Info.Max - param.Info.Min + 1);
			var items = range.Select(i => new ListItem(param.Info, i)).ToArray();
			result.SetBinding(Selector.SelectedValueProperty, Bind.To(param));
			result.SetValue(ItemsControl.ItemsSourceProperty, items);
			return result;
		}

		static Control MakeKnob(AppModel app, IThemedSubModel sub, Param param)
		{
			var result = new Knob();
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.ToolTip = Tooltip(app.Track.Synth, param);
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => ExactUI.Show(app.Settings, sub.Group, param);
			result.Sensitivity = Math.Max(Knob.MinSensitivityHint, param.Info.Max - param.Info.Min);
			return result;
		}
	}
}