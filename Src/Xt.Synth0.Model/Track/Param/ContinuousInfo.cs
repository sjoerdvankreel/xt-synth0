namespace Xt.Synth0.Model
{
	class ContinuousInfo : ParamInfo
	{
		readonly int _default;
		public override int Min => 0;
		public override int Max => 255;
		public override bool IsToggle => false;
		public override int Default => _default;

		internal ContinuousInfo(string name, string detail, int @default)
		: base(name, detail) => _default = @default;
		public override string Format(int value)
		=> (((double)value - Min) / (Max - Min)).ToString("P1");
	}
}