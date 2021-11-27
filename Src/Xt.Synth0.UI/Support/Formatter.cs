using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class Formatter : ValueConverter<int, string>
	{
		readonly ParamInfo _info;
		internal Formatter(ParamInfo info) => _info = info;
		internal override string Convert(int value)
		=> _info.Format(value).PadRight(_info.MaxDisplayLength, ' ');
	}
}