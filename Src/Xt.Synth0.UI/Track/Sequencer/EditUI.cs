using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class EditUI
	{
		internal static GroupBox Make(AppModel model, INamedModel group)
		{
			var content = GroupUI.MakeContent(model, group);
			content.VerticalAlignment = VerticalAlignment.Center;
			var result = Create.Group(group.Name, content);
			var binding = Bind.To(model.Stream, nameof(StreamModel.IsRunning),
				new VisibilityConverter(true, false));
			result.SetBinding(UIElement.VisibilityProperty, binding);
			return result;
		}
	}
}