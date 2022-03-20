using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternModel : IUIGroupContainerModel
	{
        public int Index => 0;
        public ThemeGroup ThemeGroup => ThemeGroup.Pattern;

        public string Info => null;
        public string Name => "Pattern";
        public string Id => "038215F4-C1AD-49EB-AE35-042956A834F6";
        public IReadOnlyList<IGroupContainerModel> Children => Rows;
        public IReadOnlyList<IParamGroupModel> Groups => new IParamGroupModel[0];
        public void* Address(void* parent) => &((SequencerModel.Native*)parent)->pattern;
        
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native 
        { 
            internal fixed byte rows[SequencerConfig.TotalRows * PatternRowModel.Native.Size]; 
        }

		internal PatternModel()
		{
			for (int r = 0; r < SequencerConfig.TotalRows; r += 4)
				Rows[r].Keys[0].Note.Value = (int)PatternNote.C;
		}

		public IReadOnlyList<PatternRowModel> Rows = new ReadOnlyCollection<PatternRowModel>(MakeRows());
		static IList<PatternRowModel> MakeRows() => Enumerable.Range(0, SequencerConfig.TotalRows).Select(i => new PatternRowModel(i)).ToList();
	}
}