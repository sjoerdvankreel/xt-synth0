﻿using System.Collections.Generic;
using System.Linq;

namespace Xt.Synth0.Model
{
	public class PatternModel : GroupModel<PatternModel>
	{
		public const int Length = 32;

		public PatternModel()
		{
			for (int i = 0; i < Length; i += 4)
				Rows[i].Note.Value = (int)RowNote.C;
		}

		public IList<RowModel> Rows { get; } = new List<RowModel>(
			Enumerable.Range(0, Length).Select(_ => new RowModel()));
		public override Param[][] Params() => new[] { Rows.SelectMany(n => n.Params()).ToArray() };
	}
}