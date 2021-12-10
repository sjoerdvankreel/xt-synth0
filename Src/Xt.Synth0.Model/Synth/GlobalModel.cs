namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : GroupModel
	{
		static readonly ParamInfo BpmInfo = new DiscreteInfo(
			nameof(Bpm), "Tempo", 1, 255, 120);
		public Param Bpm { get; } = new(BpmInfo);

		internal GlobalModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() 
		=> new[] { new[] { Bpm } };
	}
}