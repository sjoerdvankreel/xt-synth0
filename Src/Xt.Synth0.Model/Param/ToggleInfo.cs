namespace Xt.Synth0.Model
{
	internal sealed class ToggleInfo : ParamInfo
	{
		public override int Min => 0;
		public override int Max => 1;
		public override int Default => 0;
		public override bool IsToggle => true;

		internal ToggleInfo(string name, string detail) : 
		base(name, detail) { }
		public override string Format(int value)
		=> value == 0 ? "Off" : "On";
	}
}