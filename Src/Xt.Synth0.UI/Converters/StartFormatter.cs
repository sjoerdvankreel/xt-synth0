namespace Xt.Synth0.UI
{
	class StartFormatter : Converter<bool, string>
	{
		protected override string Convert(bool stopped)
		=> stopped ? "Start" : "Resume";
	}
}