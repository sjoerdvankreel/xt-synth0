using System.Windows.Media;

namespace Xt.Synth0.UI
{
	class EnableRowConverter : Converter<int, Brush>
	{
		readonly int _row;
		internal EnableRowConverter(int row) => _row = row;
		protected override Brush Convert(int rows) => _row < rows ? Brushes.White : Brushes.Black;
	}
}