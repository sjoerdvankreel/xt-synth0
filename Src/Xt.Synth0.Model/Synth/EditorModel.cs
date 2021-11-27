namespace Xt.Synth0.Model
{
	public sealed class EditorModel : GroupModel
	{
		const string FxDetail = "Effect count";
		const string KeysDetail = "Note count";
		const string PatsDetail = "Pattern count";
		const string EditDetail = "Active pattern";

		static readonly ParamInfo FxInfo = new DiscreteInfo(
			nameof(Fx), FxDetail, 0, 2, 1);
		static readonly ParamInfo KeysInfo = new DiscreteInfo(
			nameof(Keys), KeysDetail, 1, 3, 1);
		static readonly ParamInfo EditInfo = new DiscreteInfo(
			nameof(Edit), EditDetail, 1, PatternModel.PatternCount, 1);
		static readonly ParamInfo PatsInfo = new DiscreteInfo(
			nameof(Pats), PatsDetail, 1, PatternModel.PatternCount, 1);

		public Param Fx { get; } = new(FxInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Pats { get; } = new(PatsInfo);

		internal override Param[][] ListParamGroups() => new[] {
			new [] { Keys, Fx },
			new [] { Pats, Edit }
		};
	}
}