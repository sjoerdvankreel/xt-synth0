namespace Xt.Synth0.Model
{
	public sealed class EditorModel : GroupModel
	{
		static readonly ParamInfo AmpInfo = new ToggleInfo(nameof(Amp));
		static readonly ParamInfo FxInfo = new DiscreteInfo(nameof(Fx), 0, 2, 1);
		static readonly ParamInfo KeysInfo = new DiscreteInfo(nameof(Keys), 1, 3, 1);
		static readonly ParamInfo EditInfo = new DiscreteInfo(nameof(Edit), 1, 8, 1);
		static readonly ParamInfo PatsInfo = new DiscreteInfo(nameof(Pats), 1, 8, 1);

		public Param Fx { get; } = new(FxInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Pats { get; } = new(PatsInfo);

		internal override Param[][] ListParamGroups() => new[] {
			new [] { Amp },
			new [] { Keys, Fx },
			new [] { Pats, Edit }
		};
	}
}