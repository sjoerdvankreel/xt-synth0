using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class ParamFormatter : Converter<int, string>
	{
		readonly ParamInfo _info;
		internal ParamFormatter(ParamInfo info) => _info = info;
		protected override string Convert(int value) => _info.Format(value).PadRight(_info.MaxDisplayLength);
	}
}