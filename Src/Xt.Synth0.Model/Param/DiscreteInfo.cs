namespace Xt.Synth0.Model
{
	class DiscreteInfo : ParamInfo
	{
		readonly int _min;
		readonly int _max;
		readonly int _default;

		public override int Min => _min;
		public override int Max => _max;
		public override bool IsToggle => false;
		public override int Default => _default;

		internal DiscreteInfo(string name, string detail, int min, int max, int @default)
		: base(name, detail) => (_min, _max, _default) = (min, max, @default);
		public override string Format(int value) => value.ToString();
	}
}