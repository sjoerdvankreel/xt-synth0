using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public class PatternModel : GroupModel<PatternModel>
	{
		public const int Length = 64;

		public IList<RowModel> Rows { get; } = new List<RowModel>(
			Enumerable.Range(0, Length).Select(_ => new RowModel()));

		public PatternModel()
		{
			for (int i = 0; i < Length; i += 4)
				Rows[i].Note.Value = NoteModel.C;
		}

		public override Param<bool>[] BoolParams() => new Param<bool>[0];
		public override Param<int>[] IntParams() => Rows.SelectMany(n => n.IntParams()).ToArray();
	}
}