using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class AmpModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int a, d, s, r, lvl; }

		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param S { get; } = new(SInfo);
		public Param R { get; } = new(RInfo);
		public Param Lvl { get; } = new(LvlInfo);

		public string Name => "Amp";
		public IReadOnlyList<Param> Params => new[] { A, D, S, R, Lvl };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->amp;

		static readonly ParamInfo LvlInfo = new ContinuousInfo(p => &((Native*)p)->lvl, nameof(Lvl), "Volume", 128);
		static readonly ParamInfo SInfo = new ContinuousInfo(p => &((Native*)p)->s, nameof(S), "Sustain level", 255);
		static readonly ParamInfo DInfo = new LogInfo(p => &((Native*)p)->d, nameof(D), "Decay time", 0, 3000, "ms", "s");
		static readonly ParamInfo AInfo = new LogInfo(p => &((Native*)p)->a, nameof(A), "Attack time", 0, 1000, "ms", "s");
		static readonly ParamInfo RInfo = new LogInfo(p => &((Native*)p)->r, nameof(R), "Release time", 0, 10000, "ms", "s");
	}
}