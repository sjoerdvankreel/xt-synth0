namespace Xt.Synth0.Model
{
	public sealed class EditModel : GroupModel
	{
		static readonly ParamInfo FxInfo = new DiscreteInfo(
			nameof(Fx), "Effect count", 0, PatternRow.MaxFxCount, 1);
		static readonly ParamInfo KeysInfo = new DiscreteInfo(
			nameof(Keys), "Note count", 1, PatternRow.MaxKeyCount, 2);
		static readonly ParamInfo PatsInfo = new DiscreteInfo(
			nameof(Pats), "Pattern count", 1, PatternModel.PatternCount, 1);
		static readonly ParamInfo ActInfo = new DiscreteInfo(
			nameof(Act), "Active pattern", 1, PatternModel.PatternCount, 1);

		public Param Fx { get; } = new(FxInfo);
		public Param Act { get; } = new(ActInfo);
		public Param Pats { get; } = new(PatsInfo);
		public Param Keys { get; } = new(KeysInfo);

		public override bool Automation() => false;
		internal EditModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() => new[] {
			new [] { Keys, Fx },
			new [] { Pats, Act }
		};
	}
}