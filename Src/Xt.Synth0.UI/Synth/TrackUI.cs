using System.Windows;
using System.Windows.Controls;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal static class TrackUI
	{
		internal static UIElement Make(TrackModel model, string name, 
			int row, int column, int rowSpan = 1, int columnSpan = 1)
		{
			var result = UI.MakeElement<GroupBox>(row, column, rowSpan, columnSpan);
			result.Header = name;
			return result;
		}
	}
}