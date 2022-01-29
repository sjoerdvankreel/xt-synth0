using System.Linq;

namespace Xt.Synth0.UI
{
	internal class PlaceholderConverter : Converter<int, bool>
	{
		readonly int[] _show;
		internal PlaceholderConverter(params int[] show) => _show = show;
		protected override bool Convert(int value) => _show.Contains(value);
	}
}