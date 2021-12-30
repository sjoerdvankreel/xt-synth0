namespace Xt.Synth0.Model
{
	class ContinuousInfo : ParamInfo
	{
		readonly int _default;

		public override int Min => 0;
		public override int Max => 255;
		public override bool IsToggle => false;
		public override int Default => _default;

		public override string Format(int value) 
		=> (((double)value - Min) / (Max - Min)).ToString("P1");
		internal ContinuousInfo(Address address, string name, string detail, int @default) : 
		base(address, name, detail) => _default = @default;
	}
}