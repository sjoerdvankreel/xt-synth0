namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : GroupModel
	{
		const string BpmDetail = "Tempo";
		static readonly ParamInfo BpmInfo = new DiscreteInfo(
			nameof(Bpm), BpmDetail, 1, 999, 120);
		public Param Bpm { get; } = new(BpmInfo);

		internal GlobalModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() 
		=> new[] { new[] { Bpm } };
	}
}