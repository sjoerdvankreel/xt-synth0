using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public sealed class PatternFx : SubModel
	{
		const string ValueDetail = "Automation value";
		const string TargetDetail = "Automation target";

		static readonly ParamInfo ValueInfo = new ContinuousInfo(
			nameof(Value), ValueDetail, 0);
		static readonly ParamInfo TargetInfo = new ContinuousInfo(
			nameof(Target), TargetDetail, 0);

		public Param Value { get; } = new(ValueInfo);
		public Param Target { get; } = new(TargetInfo);

		internal override IEnumerable<Param> ListParams() 
		=> new[] { Target, Value };
	}
}