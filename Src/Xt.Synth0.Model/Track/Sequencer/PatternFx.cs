using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class PatternFx : ISubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int value, target; }

		public Param Value { get; } = new(ValueInfo);
		public Param Target { get; } = new(TargetInfo);

		readonly int _index;
		internal PatternFx(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Target, Value };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->fx[_index * TrackConstants.PatternFxSize];

		static readonly ParamInfo ValueInfo = new DiscreteInfo(p => &((Native*)p)->value, nameof(Value), "Automation value", 0, 255, 0);
		static readonly ParamInfo TargetInfo = new DiscreteInfo(p => &((Native*)p)->target, nameof(Target), "Automation target", 0, 255, 0);
	}
}