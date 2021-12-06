using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public sealed class PatternModel : SubModel
	{
		public const int PatternRows = 32;
		public const int PatternCount = 8;
		public const int RowCount = PatternCount * PatternRows;

		static PatternRow[] MakeRows()
		{
			var result = new PatternRow[RowCount];
			for (int r = 0; r < RowCount; r++)
				result[r] = new PatternRow();
			return result;
		}

		public IReadOnlyList<PatternRow> Rows { get; } = MakeRows();

		internal PatternModel()
		{
			for (int r = 0; r < RowCount; r += 4)
				Rows[r].Key1.Note.Value = (int)PatternNote.C;
		}

		internal override Param[] ListParams()
		{
			var result = new List<Param>();
			foreach (var row in Rows)
				result.AddRange(row.Params());
			return result.ToArray();
		}
	}
}