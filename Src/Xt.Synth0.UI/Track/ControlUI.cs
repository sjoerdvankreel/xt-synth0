using System;
using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	public static class ControlUI
	{
		public static event EventHandler Stop;
		public static event EventHandler Start;

		internal static UIElement Make(AppModel app)
		=> Create.ThemedGroup(app.Settings, app.Track.Seq.Control, MakeContent(app.Stream));

		static Button MakeButton(string content, Action execute)
		{
			var result = new Button();
			result.Content = content;
			result.Click += (s, e) => execute();
			return result;
		}

		static UIElement MakeContent(StreamModel stream)
		{
			var result = new DockPanel();
			result.LastChildFill = false;
			result.Add(MakeStop(stream), Dock.Right);
			result.Add(MakeStart(stream), Dock.Right);
			result.VerticalAlignment = VerticalAlignment.Bottom;
			result.HorizontalAlignment = HorizontalAlignment.Stretch;
			return Create.ThemedContent(result);
		}

		static UIElement MakeStop(StreamModel stream)
		{
			var result = MakeButton("Stop ", () => Stop(null, EventArgs.Empty));
			result.HorizontalContentAlignment = HorizontalAlignment.Center;
			var binding = Bind.To(stream, nameof(StreamModel.IsStopped), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			binding = Bind.To(stream, nameof(stream.IsRunning), new Formatter<bool>(r => r ? "Pause" : "Stop "));
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}

		static UIElement MakeStart(StreamModel stream)
		{
			var result = MakeButton("Start ", () => Start(null, EventArgs.Empty));
			result.HorizontalContentAlignment = HorizontalAlignment.Center;
			var binding = Bind.To(stream, nameof(StreamModel.IsRunning), new NegateConverter());
			result.SetBinding(UIElement.IsEnabledProperty, binding);
			binding = Bind.To(stream, nameof(stream.IsStopped), new Formatter<bool>(s => s ? "Start " : "Resume"));
			result.SetBinding(ContentControl.ContentProperty, binding);
			return result;
		}
	}
}