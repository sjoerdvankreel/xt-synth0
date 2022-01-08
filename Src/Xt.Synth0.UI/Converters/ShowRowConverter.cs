using System.Windows;

namespace Xt.Synth0.UI
{
	class ShowRowConverter : MultiConverter<int, int, Visibility>
	{
		readonly int _row;
		readonly int _min;
		internal ShowRowConverter(int row, int min) => (_row, _min) = (row, min);
		protected override Visibility Convert(int rows, int param)
		=> _row >= rows - 1 && param >= _min ? Visibility.Visible : Visibility.Collapsed;
	}
}