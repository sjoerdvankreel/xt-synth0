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
			var auto = _synth.AutoParams.SingleOrDefault(p => p.Index == target);
			var targetDetail = $"{auto?.Owner.Name} {auto?.Param.Info.Description}";
			result.AppendLine($"{_param.Info.Description}: {(auto != null ? targetDetail: "None")}");
			result.AppendLine("Ctrl + F to fill");
			if (auto != null)
				result.AppendLine($"Range: {auto.Param.Info.Range}");
			result.Append(PatternUI.EditHint);
			return result.ToString();
		}
	}
}