namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : GroupModel
	{
		const string BpmDetail = "Tempo";
		const string AmpDetail = "Volume";

		static readonly ParamInfo BpmInfo = new DiscreteInfo(
			nameof(Bpm), BpmDetail, 1, 999, 120);
		static readonly ParamInfo AmpInfo = new ContinuousInfo(
			nameof(Amp), AmpDetail, 128);

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		internal override Param[][] ListParamGroups() => new[] { new[] { Amp, Bpm } };
	}
}