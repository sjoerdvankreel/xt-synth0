using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
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

		internal static DockPanel MakeEmpty(Cell cell)
		{
			var result = Create.Element<DockPanel>(cell);
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		internal static UIElement Make(
			AppModel model, Param param, Cell cell)
		{
			var result = MakeEmpty(cell);
			result.Add(MakeControl(model, param), Dock.Left);
			if (param.Info.Type == ParamType.List) return result;
			result.Add(Create.Label(param.Info.Name), Dock.Left);
			if (param.Info.Type == ParamType.Toggle) return result;
			result.Add(MakeValue(param), Dock.Left);
			return result;
		}

		static UIElement MakeControl(AppModel model, Param param)
		=> param.Info.Type switch
		{
			ParamType.Lin => MakeKnob(model, param),
			ParamType.Exp => MakeKnob(model, param),
			ParamType.Quad => MakeKnob(model, param),
			ParamType.List => MakeList(model, param),
			ParamType.Toggle => MakeToggle(model, param),
			_ => throw new InvalidOperationException()
		};

		static UIElement MakeValue(Param param)
		{
			var result = new Label();
			var binding = Bind.Format(param);
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeToggle(AppModel model, Param param)
		{
			var result = new CheckBox();
			result.ToolTip = Tooltip(model.Track.Synth, param);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			return result;
		}

		static UIElement MakeKnob(AppModel model, Param param)
		{
			var result = new Knob();
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.ToolTip = Tooltip(model.Track.Synth, param);
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => ExactUI.Show(model.Settings, param);
			return result;
		}

		static UIElement MakeList(AppModel model, Param param)
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