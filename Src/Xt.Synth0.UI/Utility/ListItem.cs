using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal class ListItem
	{
		readonly ParamInfo _info;
		public int Value { get; }
		internal ListItem(ParamInfo info, int value) => (_info, Value) = (info, value);
		public override string ToString() => _info.Format(Value).PadRight(_info.MaxDisplayLength);
	}
}