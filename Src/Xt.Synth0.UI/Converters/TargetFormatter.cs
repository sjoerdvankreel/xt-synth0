using System.Linq;
using System.Text;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	internal class TargetFormatter : Converter<int, string>
	{
		readonly Param _param;
		readonly SynthModel _synth;
		internal TargetFormatter(SynthModel synth, Param param)
		=> (_synth, _param) = (synth, param);

		internal override string Convert(int value)
		{
			var result = new StringBuilder();
			var auto = _synth.AutoParams().SingleOrDefault(p => p.Index == value);
			result.AppendLine(_param.Info.Detail);
			var current = "none";
			if (auto != null)
				current = $"{auto.Owner.Name()} {auto.Param.Info.Detail}";
			result.AppendLine($"Current: {current}");
			if (auto != null)
				result.AppendLine($"Value range: {auto.Param.Info.Min} .. {auto.Param.Info.Max}");
			result.Append(PatternUI.EditHint);
			return result.ToString();
		}
	}
}