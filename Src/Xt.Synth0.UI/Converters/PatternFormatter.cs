using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class PatternFormatter : MultiConverter<bool, int, int, int, string>
	{
		protected override string Convert(bool running, int pats, int active, int row)
		=> $"{(running ? (row / SequencerConfig.MaxRows) + 1 : active)}/{pats}";
	}
}