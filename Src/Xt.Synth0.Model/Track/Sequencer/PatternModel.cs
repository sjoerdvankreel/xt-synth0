using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternModel : IUIGroupContainerModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native { internal fixed byte rows[Model.TotalRows * PatternRow.Native.Size]; }

		internal PatternModel()
		{
			for (int r = 0; r < Model.TotalRows; r += 4)
				Rows[r].Keys[0].Note.Value = (int)PatternNote.C;
		}

		public int Index => 0;
		public string Name => "Pattern";
		public ThemeGroup ThemeGroup => ThemeGroup.Pattern;
		public string Id => "038215F4-C1AD-49EB-AE35-042956A834F6";
		public IReadOnlyList<IGroupContainerModel> Children => Rows;
		public IReadOnlyList<IParamGroupModel> Groups => new IParamGroupModel[0];
		public void* Address(void* parent) => &((SeqModel.Native*)parent)->pattern;

		public IReadOnlyList<PatternRow> Rows = new ReadOnlyCollection<PatternRow>(MakeRows());
		static IList<PatternRow> MakeRows() => Enumerable.Range(0, Model.TotalRows).Select(i => new PatternRow(i)).ToList();
	}
}