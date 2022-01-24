using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternModel : IThemedContainer
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal fixed byte rows[Model.TotalRows * PatternRow.Native.Size]; }

		internal PatternModel()
		{
			for (int r = 0; r < Model.TotalRows; r += 4)
				Rows[r].Keys[0].Note.Value = (int)PatternNote.C;
		}

		public string Name => "Pattern";
		public ThemeGroup Group => ThemeGroup.Pattern;
		public IReadOnlyList<IModelContainer> SubContainers => Rows;
		public IReadOnlyList<ISubModel> SubModels => new ISubModel[0];
		public void* Address(void* parent) => &((SeqModel.Native*)parent)->pattern;

		public IReadOnlyList<PatternRow> Rows = new ReadOnlyCollection<PatternRow>(MakeRows());
		static IList<PatternRow> MakeRows() => Enumerable.Range(0, Model.TotalRows).Select(i => new PatternRow(i)).ToList();
	}
}