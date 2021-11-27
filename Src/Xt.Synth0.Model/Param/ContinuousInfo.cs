namespace Xt.Synth0.Model
{
	internal sealed class ContinuousInfo : ParamInfo
	{
		readonly int _default;
		public override int Min => 0;
		public override int Max => 255;
		public override bool IsToggle => false;
		public override int Default => _default;

		internal ContinuousInfo(string name, int @default) 
		: base(name) =>_default = @default;
		public override string Format(int value) => value.ToString();
	}
}