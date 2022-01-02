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

		internal override string Convert(int target)
		{
			var result = new StringBuilder();
			var auto = _synth.AutoParams.SingleOrDefault(p => p.Index == target);
			result.AppendLine(_param.Info.Detail);
			result.AppendLine("Ctrl + F to fill");
			result.AppendLine(PatternUI.EditHint);
			var current = "none";
			if (auto != null)
				current = $"{auto.Owner.Name} {auto.Param.Info.Detail}";
			result.Append($"Current: {current}");
			if (auto != null)
			{
				result.AppendLine();
				result.Append($"Value range: {auto.Param.Info.Min} .. {auto.Param.Info.Max}");
			}
			return result.ToString();
		}
	}
}