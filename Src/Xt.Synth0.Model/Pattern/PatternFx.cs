namespace Xt.Synth0.Model
{
	public sealed class PatternFx : SubModel
	{
		static readonly ParamInfo ValueInfo = new ContinuousInfo(nameof(Value), 0);
		static readonly ParamInfo TargetInfo = new ContinuousInfo(nameof(Target), 0);

		public Param Value { get; } = new(ValueInfo);
		public Param Target { get; } = new(TargetInfo);
		internal override Param[] ListParams() => new[] { Target, Value };
	}
}