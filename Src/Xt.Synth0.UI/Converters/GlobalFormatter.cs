using System.Text;

namespace Xt.Synth0.UI
{
	class GlobalFormatter : MultiConverter<bool, bool, string>
	{
		protected override string Convert(bool clip, bool overload)
		{
			var result = new StringBuilder();
			result.Append("Global");
			if (clip) result.Append(" (Clip)");
			if (overload) result.Append(" (Overload)");
			return result.ToString();
		}
	}
}