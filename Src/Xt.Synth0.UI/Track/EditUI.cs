using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class EditUI
	{
		internal static GroupBox Make(AppModel app)
		{
			var edit = app.Track.Seq.Edit;
			var result = SubUI.Make(app, edit);
			var conv = new VisibilityConverter<bool>(true, false);
			var binding = Bind.To(app.Stream, nameof(StreamModel.IsRunning), conv);
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}
	}
}