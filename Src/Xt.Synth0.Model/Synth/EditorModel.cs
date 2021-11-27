namespace Xt.Synth0.Model
{
	public sealed class EditorModel : GroupModel
	{
		static readonly ParamInfo AmpInfo = new ToggleInfo(nameof(Amp));
		static readonly ParamInfo KeysInfo = new DiscreteInfo(nameof(Keys), 1, 3, 1);
		static readonly ParamInfo EditInfo = new DiscreteInfo(nameof(Edit), 1, 4, 1);
		static readonly ParamInfo EffectsInfo = new DiscreteInfo(nameof(Effects), 0, 2, 0);
		static readonly ParamInfo PatternsInfo = new DiscreteInfo(nameof(Patterns), 1, 4, 1);

		public Param Amp { get; } = new(AmpInfo);
		public Param Keys { get; } = new(KeysInfo);
		public Param Edit { get; } = new(EditInfo);
		public Param Effects { get; } = new(EffectsInfo);
		public Param Patterns { get; } = new(PatternsInfo);

		internal override Param[][] ListParamGroups() => new[] {
			new [] { Amp },
			new [] { Keys, Effects },
			new [] { Patterns, Edit }
		};
	}
}