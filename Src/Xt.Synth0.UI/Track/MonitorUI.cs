using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class MonitorUI
	{
		internal static UIElement Make(AppModel app)
		{
			var monitor = app.Track.Seq.Monitor;
			var result = Create.ThemedGroup(app.Settings, monitor, MakeBorder(app.Stream));
			var binding = Bind.To(app.Stream, nameof(app.Stream.IsRunning),
				new VisibilityConverter(true, true));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}

		static UIElement MakeBorder(StreamModel stream)
		{
			var result = new Border();
			result.Child = MakeContent(stream);
			result.VerticalAlignment = VerticalAlignment.Stretch;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			result.SetResourceReference(Control.BackgroundProperty, Utility.BackgroundParamKey);
			return result;
		}

		static UIElement MakeContent(StreamModel stream)
		{
			var result = Create.Grid(3, 2);
			result.Add(CreateBuffer(stream, new(0, 0)));
			result.Add(CreateOverload(stream, new(1, 0)));
			result.Add(CreateClip(stream, new(1, 1)));
			result.Add(CreateCpuUsage(stream, new(2, 0)));
			result.Add(CreateGC(stream, new(2, 1)));
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Center;
			return result;
		}

		static UIElement CreateClip(StreamModel stream, Cell cell)
		{
			var result = Create.Text("Clip", cell);
			var binding = Bind.To(stream, nameof(stream.IsClipping));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateOverload(StreamModel stream, Cell cell)
		{
			var result = Create.Text("Overload", cell);
			var binding = Bind.To(stream, nameof(stream.IsOverloaded));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateBuffer(StreamModel stream, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var latencyBinding = Bind.To(stream, nameof(stream.LatencyMs));
			var bufferBinding = Bind.To(stream, nameof(stream.BufferSizeFrames));
			var binding = Bind.To(new MonitorFormatter(), bufferBinding, latencyBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
			result.SetValue(Grid.ColumnSpanProperty, 2);
			return result;
		}

		static UIElement CreateGC(StreamModel stream, Cell cell)
		{
			var result = Create.Element<WrapPanel>(cell);
			result.Add(Create.Text("GC "));
			result.Add(CreateGCGeneration(stream, nameof(stream.GC0Collected), "0"));
			result.Add(CreateGCGeneration(stream, nameof(stream.GC1Collected), " 1"));
			result.Add(CreateGCGeneration(stream, nameof(stream.GC2Collected), " 2"));
			return result;
		}

		static UIElement CreateGCGeneration(StreamModel stream, string path, string generation)
		{
			var result = new TextBlock();
			result.Text = generation;
			var binding = Bind.To(stream, path);
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateCpuUsage(StreamModel stream, Cell cell)
		{
			var result = Create.Element<WrapPanel>(cell);
			result.Add(Create.Text("CPU "));
			var text = Create.Element<TextBlock>(cell);
			var binding = Bind.To(stream, nameof(stream.CpuUsage), new Formatter<double>(u => u.ToString("P1")));
			text.SetBinding(TextBlock.TextProperty, binding);
			result.Add(text);
			result.Add(Create.Text(" "));
			return result;
		}
	}
}