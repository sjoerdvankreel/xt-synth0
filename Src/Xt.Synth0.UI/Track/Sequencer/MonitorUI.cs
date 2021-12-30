using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class MonitorUI
	{
		internal static UIElement Make(StreamModel model)
		{
			var result = Create.Group("Monitor", MakeContent(model));
			var binding = Bind.To(model, nameof(model.IsRunning), 
				new VisibilityConverter(true, true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeContent(StreamModel model)
		{
			var result = Create.Grid(4, 2);
			result.Add(Create.Text("CPU ", new(0, 0)));
			result.Add(CreateCpuUsage(model, new(0, 1)));
			result.Add(Create.Text("GC ", new(1, 0)));
			result.Add(CreateGC(model, new(1, 1)));
			result.Add(CreateClip(model, new(2, 0)));
			result.Add(CreateOverload(model, new(2, 1)));
			result.Add(Create.Text("Buffer ", new(3, 0)));
			result.Add(CreateBuffer(model, new(3, 1)));
			return result;
		}

		static UIElement CreateClip(StreamModel model, Cell cell)
		{
			var result = Create.Text("Clip", cell);
			var binding = Bind.To(model, nameof(model.IsClipping));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateOverload(StreamModel model, Cell cell)
		{
			var result = Create.Text("Overload", cell);
			var binding = Bind.To(model, nameof(model.IsOverloaded));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateCpuUsage(StreamModel model, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var binding = Bind.To(model, nameof(model.CpuUsage), new CpuUsageFormatter());
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}

		static UIElement CreateBuffer(StreamModel model, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var latencyBinding = Bind.To(model, nameof(model.LatencyMs));
			var bufferBinding = Bind.To(model, nameof(model.BufferSizeFrames));
			var binding = Bind.To(new MonitorFormatter(), bufferBinding, latencyBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}

		static UIElement CreateGC(StreamModel model, Cell cell)
		{
			var result = Create.Element<WrapPanel>(cell);
			result.Add(CreateGCGeneration(model, nameof(model.GC0Collected), "0"));
			result.Add(CreateGCGeneration(model, nameof(model.GC1Collected), " 1"));
			result.Add(CreateGCGeneration(model, nameof(model.GC2Collected), " 2"));
			return result;
		}

		static UIElement CreateGCGeneration(StreamModel model, string path, string generation)
		{
			var result = new TextBlock();
			result.Text = generation;
			var binding = Bind.To(model, path);
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}
	}
}