namespace Xt.Synth0.UI
{
	class CpuUsageFormatter : Converter<double, string>
	{
		protected override string Convert(double usage) => usage.ToString("P1");
	}
}