namespace Xt.Synth0.UI
{
	class EnableRowConverter : Converter<int, bool>
	{
		readonly int _row;
		internal EnableRowConverter(int row) => _row = row;
		protected override bool Convert(int rows) => _row < rows;
	}
}