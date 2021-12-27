using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	static class GlobalUI
	{
		internal static UIElement Make(AppModel app, GlobalModel global)
		{
			var result = GroupUI.Make(app, global);
			var clipBinding = Bind.To(app.Audio, nameof(AudioModel.IsClipping));
			var overloadBinding = Bind.To(app.Audio, nameof(AudioModel.IsOverloaded));
			var binding = Bind.To(new GlobalFormatter(), clipBinding, overloadBinding);
			result.SetBinding(HeaderedContentControl.HeaderProperty, binding);
			return result;
		}
	}
}