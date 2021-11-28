namespace Xt.Synth0.Model
{
	public sealed class AmpModel : GroupModel
	{
		const string LvlDetail = "Volume";
		const string DDetail = "Decay time";
		const string ADetail = "Attack time";
		const string RDetail = "Release time";
		const string SDetail = "Sustain level";

		static readonly ParamInfo AInfo = new LogInfo(
			nameof(A), ADetail, 0, 1000, "ms", "s");
		static readonly ParamInfo DInfo = new LogInfo(
			nameof(D), DDetail, 0, 3000, "ms", "s");
		static readonly ParamInfo RInfo = new LogInfo(
			nameof(R), RDetail, 0, 10000, "ms", "s");
		static readonly ParamInfo SInfo = new ContinuousInfo(
			nameof(S), SDetail, 255);
		static readonly ParamInfo LvlInfo = new ContinuousInfo(
			nameof(Lvl), LvlDetail, 128);

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
	}
}