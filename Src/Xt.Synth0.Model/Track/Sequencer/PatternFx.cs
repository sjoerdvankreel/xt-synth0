using MessagePack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternFx : ISubModel, IStoredModel<PatternFx.Native, PatternFx.Native>
	{
		[MessagePackObject(keyAsPropertyName: true)]
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public const int Size = 8;
			public int tgt, val;
		}

		public Param Val { get; } = new(ValInfo);
		public Param Tgt { get; } = new(TgtInfo);

		readonly int _index;
		internal PatternFx(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Tgt, Val };
		public void Load(in Native stored, out Native native) => native = stored;
		public void Store(in Native native, out Native stored) => stored = native;
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->fx[_index * Native.Size];

		static readonly ParamInfo ValInfo = ParamInfo.Level(p => &((Native*)p)->val, nameof(Val), "Automation value", 0);
		static readonly ParamInfo TgtInfo = ParamInfo.Level(p => &((Native*)p)->tgt, nameof(Tgt), "Automation target", 0);
	}
}