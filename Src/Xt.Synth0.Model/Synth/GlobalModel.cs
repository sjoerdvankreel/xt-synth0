namespace Xt.Synth0.Model
{
	public class GlobalModel : IGroupModel
	{
		static readonly ParamInfo<int> BpmInfo = new(ParamType.Int, nameof(Bpm), 1, 1000, 120);
		static readonly ParamInfo<int> AmpInfo = new(ParamType.Float, nameof(Amp), 0, 256, 128);

		public Param<int> Bpm { get; } = Param.Of(BpmInfo);
		public Param<int> Amp { get; } = Param.Of(AmpInfo);

		public Param<int>[] IntParams() => new[] { Bpm, Amp };
		public Param<bool>[] BoolParams() => new Param<bool>[0];

		internal void CopyTo(GlobalModel model)
		{
			model.Bpm.Value = Bpm.Value;
			model.Amp.Value = Amp.Value;
		}
	}
}