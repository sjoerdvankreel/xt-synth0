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
			var result = new StringBuilder();
			var auto = model.AutoParam(param);
			result.AppendLine(param.Info.Detail);
			result.AppendLine($"Range: {param.Info.Min} .. {param.Info.Max}");
			result.Append("Automation target: ");
			result.AppendLine(auto?.Index.ToString("X2") ?? "none");
			result.Append("Right-click to set exact value");
			return result.ToString();
		}

		internal static void Add(
			Grid grid, AppModel model, Param param, Cell cell)
		{
			grid.Children.Add(Create.Label(param.Info.Name, cell.Right(1)));
			grid.Children.Add(MakeValue(param, cell.Right(2)));
			if (param.Info.IsToggle)
				grid.Children.Add(MakeToggle(model, param, cell));
			else
				grid.Children.Add(MakeKnob(model, param, cell));
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
			var result = Create.Element<Toggle>(cell);
			result.SetBinding(ToggleButton.IsCheckedProperty, Bind.To(param));
			result.ToolTip = Tooltip(model.Synth, param);
			result.MouseRightButtonUp += (s, e) => EditUI.Show(model.Settings, param);
			return result;
		}

		static UIElement MakeKnob(AppModel model, Param param, Cell cell)
		{
			var result = Create.Element<Knob>(cell);
			result.Minimum = param.Info.Min;
			result.Maximum = param.Info.Max;
			result.SetBinding(RangeBase.ValueProperty, Bind.To(param));
			result.MouseRightButtonUp += (s, e) => EditUI.Show(model.Settings, param);
			result.ToolTip = Tooltip(model.Synth, param);
			return result;
		}
	}
}