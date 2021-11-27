using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public sealed class PatternModel : SubModel
	{
		public const int RowCount = 32;

		static PatternRow[] MakeRows()
		{
			var result = new PatternRow[RowCount];
			for (int r = 0; r < RowCount; r++)
					result[r] = new PatternRow();
			return result;
		}

		public PatternRow[] Rows { get; } = MakeRows();

		public PatternModel()
		{
			for (int r = 0; r < RowCount; r++)
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