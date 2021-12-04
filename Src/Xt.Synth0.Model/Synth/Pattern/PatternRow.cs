using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class PatternRow : SubModel
	{
		public PatternFx Fx1 { get; } = new();
		public PatternFx Fx2 { get; } = new();
		public PatternKey Key1 { get; } = new();
		public PatternKey Key2 { get; } = new();
		public PatternKey Key3 { get; } = new();

		internal override Param[] ListParams() => new[] {
			Fx1.Params(), Fx2.Params(),
			Key1.Params(), Key2.Params(), Key3.Params(),
		}.SelectMany(p => p).ToArray();
	}
}