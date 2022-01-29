using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternFx : IParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 8;
			internal int tgt, val;
		}

		public Param Val { get; } = new(ValInfo);
		public Param Tgt { get; } = new(TgtInfo);
		internal PatternFx(int index) => Index = index;

		public int Index { get; }
		public IReadOnlyList<Param> Params => new[] { Tgt, Val };
		public string Id => "ABD763E7-8A06-4582-8D24-88214BB04A3A";
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->fx[Index * Native.Size];

		static readonly ParamInfo ValInfo = ParamInfo.Level(p => &((Native*)p)->val, nameof(Val), "Automation value", 0);
		static readonly ParamInfo TgtInfo = ParamInfo.Level(p => &((Native*)p)->tgt, nameof(Tgt), "Automation target", 0);
	}
}