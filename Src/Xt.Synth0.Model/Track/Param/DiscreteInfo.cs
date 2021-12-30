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

		public override string Format(int value) => value.ToString();
		internal DiscreteInfo(Address address, string name, string detail, int min, int max, int @default) :
		base(address, name, detail) => (_min, _max, _default) = (min, max, @default);
	}
}