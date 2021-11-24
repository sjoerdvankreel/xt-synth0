namespace Xt.Synth0.UI
{
	internal class GridSettings
	{
		internal int Row { get; }
		internal int Col { get; }
		internal int RowSpan { get; }
		internal int ColSpan { get; }

		internal GridSettings(int row, int col, int rowSpan = 1, int colSpan = 1)
		=> (Row, Col, RowSpan, ColSpan) = (row, col, rowSpan, colSpan);
	}
}