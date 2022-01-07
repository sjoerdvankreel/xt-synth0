using System.Linq;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal class ListItem
	{
		public int Value { get; }
		readonly string[] _displays;
		public override string ToString() => _displays[Value];

		internal ListItem(ParamInfo info, int value)
		{
			Value = value;
			var allDisplays = Enumerable.Range(info.Min, info.Max - info.Min + 1);
			var maxLength = allDisplays.Max(d => info.Format(d).Length);
			_displays = allDisplays.Select(d => info.Format(d).PadRight(maxLength)).ToArray();
		}
	}
}