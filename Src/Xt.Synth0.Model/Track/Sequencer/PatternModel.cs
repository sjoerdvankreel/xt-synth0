using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternModel : IModelGroup
	{
		public const int BeatRows = 4;
		public const int PatternRows = 32;
		public const int PatternCount = 8;
		public const int RowCount = PatternCount * PatternRows;

		static PatternModel()
		{
			if (Size != XtsPatternModelSize())
				throw new InvalidOperationException();
		}

		internal const int Size = 1;
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsPatternModelSize();
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native { internal fixed byte rows[RowCount * PatternRow.Size]; }

		internal PatternModel()
		{
			for (int r = 0; r < RowCount; r += 4)
				Rows[r].Keys[0].Note.Value = (int)PatternNote.C;
		}

		public IReadOnlyList<IModelGroup> SubGroups => Rows;
		public IReadOnlyList<ISubModel> SubModels => new ISubModel[0];
		public void* Address(void* parent) => &((SequencerModel.Native*)parent)->pattern;

		public IReadOnlyList<PatternRow> Rows = new ReadOnlyCollection<PatternRow>(MakeRows());
		static IList<PatternRow> MakeRows() => Enumerable.Repeat(0, RowCount).Select(i => new PatternRow(i)).ToList();
	}
}