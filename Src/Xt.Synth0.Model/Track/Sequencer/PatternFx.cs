using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternFx : ISubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int target, value; }

		public Param Value { get; } = new(ValueInfo);
		public Param Target { get; } = new(TargetInfo);

		readonly int _index;
		internal PatternFx(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Target, Value };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->fx[_index * TrackConstants.PatternFxSize];

		static readonly ParamInfo ValueInfo = ParamInfo.Lin(p => &((Native*)p)->value, nameof(Value), false, 0, 255, 0);
		static readonly ParamInfo TargetInfo = ParamInfo.Lin(p => &((Native*)p)->target, nameof(Target), false, 0, 255, 0);
	}
}