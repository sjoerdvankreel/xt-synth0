using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class MonitorUI
	{
		internal static UIElement Make(AudioModel model)
		{
			var result = Create.Group("Monitor", MakeContent(model));
			var binding = Bind.To(model, nameof(model.IsRunning), new VisibilityConverter(true, true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeContent(AudioModel model)
		{
			var result = Create.Grid(3, 2);
			result.Add(Create.Text("Cpu ", new(0, 0)));
			result.Add(CreateCpuUsage(model, new(0, 1)));
			result.Add(Create.Text("Buffer ", new(1, 0)));
			result.Add(CreateBuffer(model, new(1, 1)));
			result.Add(CreateClip(model, new(2, 0)));
			result.Add(CreateOverload(model, new(2, 1)));
			return result;
		}

		static UIElement CreateClip(AudioModel model, Cell cell)
		{
			var result = Create.Text("Clip", cell);
			var binding = Bind.To(model, nameof(model.IsClipping));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateOverload(AudioModel model, Cell cell)
		{
			var result = Create.Text("Overload", cell);
			var binding = Bind.To(model, nameof(model.IsOverloaded));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateCpuUsage(AudioModel model, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var binding = Bind.To(model, nameof(model.CpuUsage), new CpuUsageFormatter());
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}

		static UIElement CreateBuffer(AudioModel model, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var latencyBinding = Bind.To(model, nameof(model.LatencyMs));
			var bufferBinding = Bind.To(model, nameof(model.BufferSizeFrames));
			var binding = Bind.To(new MonitorFormatter(), bufferBinding, latencyBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}
	}
}