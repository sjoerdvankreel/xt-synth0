using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class EditUI
	{
		internal static GroupBox Make(AppModel app)
		{
			var edit = app.Track.Sequencer.Edit;
			var content = SubUI.MakeContent(app, edit);
			var result = Create.ThemedGroup(app.Settings, edit, content);
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsRunning),
				new VisibilityConverter(true, false));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}
	}
}