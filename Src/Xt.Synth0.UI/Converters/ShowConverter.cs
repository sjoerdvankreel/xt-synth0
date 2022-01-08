using System.Windows;

namespace Xt.Synth0.UI
{
	class ShowConverter : Converter<int, Visibility>
	{
		readonly int _min;
		internal ShowConverter(int min) => _min = min;
		protected override Visibility Convert(int value)
		=> value >= _min ? Visibility.Visible : Visibility.Collapsed;
	}
}