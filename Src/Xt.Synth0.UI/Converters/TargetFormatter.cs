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
			var param = _synth.AutoParams[target];
			result.AppendLine(_param.Info.Detail);
			result.AppendLine("Ctrl + F to fill");
			result.AppendLine(PatternUI.EditHint);
			var current = $"{param.Owner.Name} {param.Param.Info.Detail}";
			result.Append($"Current: {current}");
			result.AppendLine();
			result.Append($"Value range: {param.Param.Info.Min} .. {param.Param.Info.Max}");
			return result.ToString();
		}
	}
}