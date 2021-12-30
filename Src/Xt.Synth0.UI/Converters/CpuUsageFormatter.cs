namespace Xt.Synth0.UI
{
	class CpuUsageFormatter : Converter<double, string>
	{
		internal override string Convert(double usage) => usage.ToString("P1");
	}
}