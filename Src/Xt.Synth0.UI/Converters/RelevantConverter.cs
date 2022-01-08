using System.Windows;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal class RelevantConverter : Converter<int, Visibility>
	{
		readonly Param _param;
		internal RelevantConverter(Param param) 
		=> _param = param;
		protected override Visibility Convert(int value) 
		=> _param.Info.IsRelevant(value) ? Visibility.Visible : Visibility.Hidden;
	}
}