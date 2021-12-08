using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class PatternFormatter : MultiConverter<bool, int, int, int, string>
	{
		protected override string Convert(bool running, int edit, int pats, int row)
		=> $"{(running ? (row / PatternModel.PatternRows) + 1 : edit)}/{pats}";
	}
}