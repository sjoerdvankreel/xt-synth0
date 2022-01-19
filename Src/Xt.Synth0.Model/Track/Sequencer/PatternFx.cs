using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternFx : ISubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native
		{
			internal const int Size = 8;
			internal int tgt, val;
		}

		public Param Val { get; } = new(ValInfo);
		public Param Tgt { get; } = new(TgtInfo);

		readonly int _index;
		internal PatternFx(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Tgt, Val };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->fx[_index * Native.Size];

		static readonly ParamInfo ValInfo = ParamInfo.Level(p => &((Native*)p)->val, nameof(Val), "Automation value", false, 0);
		static readonly ParamInfo TgtInfo = ParamInfo.Level(p => &((Native*)p)->tgt, nameof(Tgt), "Automation target", false, 0);
	}
}