namespace Xt.Synth0.UI
{
	class NegateConverter : Converter<bool, bool>
	{
		internal override bool Convert(bool value) => !value;
		internal override bool ConvertBack(bool value) => !value;
	}
}