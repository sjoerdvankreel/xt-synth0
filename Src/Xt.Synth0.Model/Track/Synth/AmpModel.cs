namespace Xt.Synth0.Model
{
	public sealed class AmpModel : GroupModel
	{
		internal struct Native
		{
			internal int a;
			internal int d;
			internal int s;
			internal int r;
			internal int lvl;
		}

		static readonly ParamInfo DInfo = new LogInfo(
			nameof(D), "Decay time", 0, 3000, "ms", "s");
		static readonly ParamInfo AInfo = new LogInfo(
			nameof(A), "Attack time", 0, 1000, "ms", "s");
		static readonly ParamInfo RInfo = new LogInfo(
			nameof(R), "Release time", 0, 10000, "ms", "s");
		static readonly ParamInfo SInfo = new ContinuousInfo(
			nameof(S), "Sustain level", 255);
		static readonly ParamInfo LvlInfo = new ContinuousInfo(
			nameof(Lvl), "Volume", 128);

		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param S { get; } = new(SInfo);
		public Param R { get; } = new(RInfo);
		public Param Lvl { get; } = new(LvlInfo);

		internal AmpModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() => new[]
		{
			new[] { Lvl },
			new[] { A, D },
			new[] { S, R },
		};

		internal void ToNative(ref Native native)
		{
			native.a = A.Value;
			native.d = D.Value;
			native.s = S.Value;
			native.r = R.Value;
			native.lvl = Lvl.Value;
		}

		internal void FromNative(ref Native native)
		{
			A.Value = native.a;
			D.Value = native.d;
			S.Value = native.s;
			R.Value = native.r;
			Lvl.Value = native.lvl;
		}
	}
}