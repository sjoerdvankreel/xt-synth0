namespace Xt.Synth0.UI
{
	class StopFormatter : Converter<bool, string>
	{
		protected override string Convert(bool running) 
		=> running ? "Pause" : "Stop";
	}
}