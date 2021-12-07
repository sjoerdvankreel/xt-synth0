using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class PatternModel : SubModel
	{
		public const int PatternRows = 32;
		public const int PatternCount = 8;
		public const int RowCount = PatternCount * PatternRows;

		static IEnumerable<PatternRow> MakeRows() 
		=> Enumerable.Repeat(0, RowCount).Select(_ => new PatternRow());

		[JsonIgnore]
		public IReadOnlyList<PatternRow> Rows => _rows.Items;
		[JsonProperty(nameof(Rows))]
		readonly ModelList<PatternRow> _rows = new(MakeRows());

		internal PatternModel()
		{
			for (int r = 0; r < RowCount; r += 4)
				Rows[r].Keys[0].Note.Value = (int)PatternNote.C;
		}

		internal override IEnumerable<Param> ListParams()
		=> Rows.SelectMany(r => r.Params());
	}
}