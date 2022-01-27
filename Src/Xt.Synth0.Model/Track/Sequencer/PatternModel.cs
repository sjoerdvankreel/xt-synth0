using MessagePack;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternModel : IThemedContainer, IStoredModel<PatternModel.Native, PatternModel.Stored>
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native { public fixed byte rows[Model.TotalRows * PatternRow.Native.Size]; }
		[MessagePackObject(keyAsPropertyName: true)]
		public struct Stored { public PatternRow.Stored[] rows = new PatternRow.Stored[Model.TotalRows]; }

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

		public void Store(ref Native native, ref Stored stored)
		{
			fixed (byte* rows = native.rows)
				for (int i = 0; i < Model.TotalRows; i++)
					Rows[i].Store(ref ((PatternRow.Native*)rows)[i], ref stored.rows[i]);
		}

		public void Load(ref Stored stored, ref Native native)
		{
			fixed (byte* rows = native.rows)
				for (int i = 0; i < Model.TotalRows && i < stored.rows.Length; i++)
					Rows[i].Load(ref stored.rows[i], ref ((PatternRow.Native*)rows)[i]);
		}
	}
}