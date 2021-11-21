﻿using System.Collections.ObjectModel;
using System.Linq;

namespace Xt.Synth0.Model
{
	public class PatternModel : GroupModel<PatternModel>
	{
		public const int Length = 32;

		public ReadOnlyCollection<RowModel> Rows { get; }
			= new ReadOnlyCollection<RowModel>(Enumerable
				.Range(0, Length).Select(_ => new RowModel()).ToList());

		public PatternModel()
		{
			for (int i = 0; i < Length; i += 4)
				Rows[i].Note.Value = NoteModel.C;
		}

		public override Param<bool>[] BoolParams() => new Param<bool>[0];
		public override Param<int>[] IntParams() => Rows.SelectMany(n => n.IntParams()).ToArray();
	}
}