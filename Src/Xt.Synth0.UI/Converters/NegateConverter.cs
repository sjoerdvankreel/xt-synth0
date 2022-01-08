namespace Xt.Synth0.UI
{
	class NegateConverter : Converter<bool, bool>
	{
		protected override bool Convert(bool value) => !value;
		protected override bool ConvertBack(bool value) => !value;
	}
}