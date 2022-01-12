using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class PatternFormatter : MultiConverter<bool, int, int, int, string>
	{
		readonly string _header;
		internal PatternFormatter(string header) => _header = header;
		internal override string Convert(bool running, int pats, int active, int row)
		=> $"{_header} {(running ? (row / TrackConstants.MaxRows) + 1 : active)}/{pats}";
	}
}