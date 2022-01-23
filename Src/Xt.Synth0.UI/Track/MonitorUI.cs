using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class MonitorUI
	{
		internal static UIElement Make(AppModel app)
		=> Create.ThemedGroup(app.Settings, app.Track.Seq.Monitor, MakeBorder(app.Stream));

		static UIElement MakeBorder(StreamModel stream)
		{
			var result = new Border();
			result.Padding = new(2.0);
			result.Child = MakeContent(stream);
			result.VerticalAlignment = VerticalAlignment.Stretch;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			return Create.ThemedContent(result);
		}

		static UIElement MakeContent(StreamModel stream)
		{
			var result = Create.Grid(3, 2);
			result.Add(CreateCpuUsage(stream, new(0, 0)));
			result.Add(CreateGC(stream, new(0, 1)));
			result.Add(CreateClipOverload(stream, new(1, 0)));
			result.Add(CreateExhausted(stream, new(1, 1)));
			result.Add(CreateBuffer(stream, new(2, 0)));
			result.Add(CreateVoices(stream, new(2, 1)));
			result.VerticalAlignment = VerticalAlignment.Center;
			result.HorizontalAlignment = HorizontalAlignment.Left;
			return result;
		}

		static UIElement CreateExhausted(StreamModel stream, Cell cell)
		{
			var result = Create.Text("Exhausted", cell);
			var binding = Bind.To(stream, nameof(stream.IsExhausted));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateClipOverload(StreamModel model, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Add(CreateClip(model));
			result.Add(CreateOverload(model));
			return result;
		}

		static UIElement CreateClip(StreamModel stream)
		{
			var result = Create.Text("Clip ");
			var binding = Bind.To(stream, nameof(stream.IsClipping));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateOverload(StreamModel stream)
		{
			var result = Create.Text("Overload ");
			var binding = Bind.To(stream, nameof(stream.IsOverloaded));
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			return result;
		}

		static UIElement CreateVoices(StreamModel stream, Cell cell)
		{
			var result = Create.Element<StackPanel>(cell);
			result.Orientation = Orientation.Horizontal;
			result.Add(Create.Text("Voices: "));
			var text = result.Add(new TextBlock());
			var binding = Bind.To(stream, nameof(stream.Voices));
			text.SetBinding(TextBlock.TextProperty, binding);
			return result;
		}

		static UIElement CreateBuffer(StreamModel stream, Cell cell)
		{
			var result = Create.Element<TextBlock>(cell);
			var latencyBinding = Bind.To(stream, nameof(stream.LatencyMs));
			var bufferBinding = Bind.To(stream, nameof(stream.BufferSizeFrames));
			var binding = Bind.To(new MonitorFormatter(), bufferBinding, latencyBinding);
			result.SetBinding(TextBlock.TextProperty, binding);
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