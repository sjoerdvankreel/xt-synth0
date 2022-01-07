using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class MonitorUI
	{
		internal static UIElement Make(StreamModel model)
		{
			var result = Create.Group("Monitor", MakeBorder(model));
			var binding = Bind.To(model, nameof(model.IsRunning),
				new VisibilityConverter(true, true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeBorder(StreamModel model)
		{
			var result = new Border();
			result.Child = MakeContent(model);
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		static UIElement MakeContent(StreamModel model)
		{
			var result = Create.Grid(3, 2);
			result.Add(CreateBuffer(model, new(0, 0)));
			result.Add(CreateOverload(model, new(1, 0)));
			result.Add(CreateClip(model, new(1, 1)));
			result.Add(CreateCpuUsage(model, new(2, 0)));
			result.Add(CreateGC(model, new(2, 1)));
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
			var result = Create.Element<WrapPanel>(cell);
			result.Add(Create.Text("CPU "));
			var text = Create.Element<TextBlock>(cell);
			var binding = Bind.To(model, nameof(model.CpuUsage), new CpuUsageFormatter());
			text.SetBinding(TextBlock.TextProperty, binding);
			result.Add(text);
			result.Add(Create.Text(" "));
			return result;
		}

		static UIElement CreateBuffer(StreamModel model, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var latencyBinding = Bind.To(model, nameof(model.LatencyMs));
			var bufferBinding = Bind.To(model, nameof(model.BufferSizeFrames));
			var binding = Bind.To(new MonitorFormatter(), bufferBinding, latencyBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.SetValue(Grid.ColumnSpanProperty, 2);
			return result;
		}

		static UIElement CreateGC(StreamModel model, Cell cell)
		{
			var result = Create.Element<WrapPanel>(cell);
			result.Add(Create.Text("GC "));
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