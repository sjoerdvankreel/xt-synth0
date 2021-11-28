namespace Xt.Synth0.UI
{
	class PatternFormatter : MultiConverter<int, int, string>
	{
		protected override string Convert(int t, int u) => $"{t}/{u}";
	}
}