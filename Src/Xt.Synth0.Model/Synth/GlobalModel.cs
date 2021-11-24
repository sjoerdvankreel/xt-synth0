namespace Xt.Synth0.Model
{
	public class GlobalModel : GroupModel<GlobalModel>
	{
		static readonly ParamInfo BpmInfo = new(ParamType.Int, nameof(Bpm), 1, 1000, 120);
		static readonly ParamInfo AmpInfo = new(ParamType.Percent, nameof(Amp), 0, 255, 128);

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		public override Param[][] Params() => new[] { new[] { Amp, Bpm } };
	}
}