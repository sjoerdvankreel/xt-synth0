namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : GroupModel
	{
		static readonly ParamInfo AmpInfo = new ContinuousInfo(nameof(Amp), 128);
		static readonly ParamInfo BpmInfo = new DiscreteInfo(nameof(Bpm), 1, 999, 120);

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		internal override Param[][] ListParamGroups() => new[] { new[] { Bpm, Amp } };
	}
}