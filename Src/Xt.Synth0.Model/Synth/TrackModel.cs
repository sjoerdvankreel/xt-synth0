namespace Xt.Synth0.Model
{
	public sealed class TrackModel : GroupModel
	{
		static readonly ParamInfo FxInfo = new DiscreteInfo(
			nameof(Fx), "Effect count", 0, PatternRow.MaxFxCount, 1);
		static readonly ParamInfo KeysInfo = new DiscreteInfo(
			nameof(Keys), "Note count", 1, PatternRow.MaxKeyCount, 1);
		static readonly ParamInfo PatsInfo = new DiscreteInfo(
			nameof(Pats), "Pattern count", 1, PatternModel.PatternCount, 1);
		static readonly ParamInfo EditInfo = new DiscreteInfo(
			nameof(Edit), "Active pattern", 1, PatternModel.PatternCount, 1);

		public Param Fx { get; } = new(FxInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Pats { get; } = new(PatsInfo);

		public override bool Automation() => false;
		internal TrackModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() => new[] {
			new [] { Keys, Fx },
			new [] { Pats, Edit }
		};
	}
}