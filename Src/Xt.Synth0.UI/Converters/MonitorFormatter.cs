namespace Xt.Synth0.UI
{
	class MonitorFormatter : MultiConverter<int, double, string>
	{
		protected override string Convert(int frames, double latency)
		=> $"{frames} @ {latency.ToString("N1")}ms ";
	}
}