using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternModel : IModelGroup
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native { internal fixed byte rows[TrackConstants.TotalRowCount * TrackConstants.PatternRowSize]; }

		internal PatternModel()
		{
			for (int r = 0; r < TrackConstants.TotalRowCount; r += 4)
				Rows[r].Keys[0].Note.Value = (int)PatternNote.C;
		}

		public IReadOnlyList<IModelGroup> SubGroups => Rows;
		public IReadOnlyList<ISubModel> SubModels => new ISubModel[0];
		public void* Address(void* parent) => &((SequencerModel.Native*)parent)->pattern;

		public IReadOnlyList<PatternRow> Rows = new ReadOnlyCollection<PatternRow>(MakeRows());
		static IList<PatternRow> MakeRows() => Enumerable.Range(0, TrackConstants.TotalRowCount).Select(i => new PatternRow(i)).ToList();
	}
}