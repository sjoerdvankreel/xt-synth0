namespace Xt.Synth0.Model
{
	internal sealed class DiscreteInfo : ParamInfo
	{
		readonly int _min;
		readonly int _max;
		readonly int _default;

		public override int Min => _min;
		public override int Max => _max;
		public override bool IsToggle => false;
		public override int Default => _default;

		internal DiscreteInfo(string name, int min, int max, int @default)
		: base(name) => (_min, _max, _default) = (min, max, @default);
		public override string Format(int value) => value.ToString();
	}
}