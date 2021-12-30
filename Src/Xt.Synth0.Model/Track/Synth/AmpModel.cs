using System;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public sealed class AmpModel : INamedModel
	{
		internal const int NativeSize = 1;

		[StructLayout(LayoutKind.Sequential)]
		internal struct Native
		{
			internal int a;
			internal int d;
			internal int s;
			internal int r;
			internal int lvl;
		}

		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param S { get; } = new(SInfo);
		public Param R { get; } = new(RInfo);
		public Param Lvl { get; } = new(LvlInfo);

		public string Name => "Amp";
		public int Size => NativeSize;
		public Param[] Params => new[] { A, D, S, R, Lvl };

		static unsafe readonly ParamInfo AInfo = new LogInfo(p => new IntPtr(&((Native*)p)->a), nameof(A), "Attack time",  0, 1000, "ms", "s");
		static readonly ParamInfo DInfo = new LogInfo(nameof(D), "Decay time", 0, 3000, "ms", "s");
		static readonly ParamInfo SInfo = new ContinuousInfo(nameof(S), "Sustain level", 255);
		static readonly ParamInfo RInfo = new LogInfo(nameof(R), "Release time", 0, 10000, "ms", "s");
		static readonly ParamInfo LvlInfo = new ContinuousInfo(nameof(Lvl), "Volume", 128);
	}
}