using Xt.Synth0.UI;

namespace Xt.Synth0
{
	internal class TitleFormatter : MultiConverter<string, bool, string>
	{
		protected override string Convert(string title, bool dirty)
		{
			var result = title == null ? nameof(Synth0) : $"{nameof(Synth0)} ({title})";
			return dirty ? $"{result} *" : result;
		}
	}
}