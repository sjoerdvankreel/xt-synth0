using System.Linq;
using System.Text;
using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class TargetFormatter : Converter<int, string>
	{
		readonly Param _param;
		readonly SynthModel _synth;
		internal TargetFormatter(SynthModel synth, Param param)
		=> (_synth, _param) = (synth, param);

		protected override string Convert(int target)
		{
			var result = new StringBuilder();
			var param = _synth.SynthParams.SingleOrDefault(p => p.Index == target);
			var targetDetail = $"{param?.Group.Name} {param?.Param.Info.Description}";
			result.AppendLine($"{_param.Info.Description}: {(param != null ? targetDetail : "None")}");
			result.AppendLine("Ctrl + F to fill");
			if (param != null) result.AppendLine($"Range: {param.Param.Info.Range(true)}");
			result.Append(PatternUI.EditHint);
			return result.ToString();
		}
	}
}