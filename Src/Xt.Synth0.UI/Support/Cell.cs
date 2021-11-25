namespace Xt.Synth0.UI
{
	class Cell
	{
		internal int Row { get; }
		internal int Col { get; }
		internal int RowSpan { get; }
		internal int ColSpan { get; }

		internal Cell(int row, int col, int rowSpan = 1, int colSpan = 1)
		=> (Row, Col, RowSpan, ColSpan) = (row, col, rowSpan, colSpan);
		internal Cell Down(int n) => new(Row + n, Col, RowSpan, ColSpan);
		internal Cell Right(int n) => new(Row, Col + n, RowSpan, ColSpan);
	}
}