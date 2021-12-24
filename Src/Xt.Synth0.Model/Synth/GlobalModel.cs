using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : GroupModel
	{
		public static readonly string[] Methods = Enum.
			GetValues<SynthMethod>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo BpmInfo = new DiscreteInfo(
			nameof(Bpm), "Tempo", 1, 255, 120);
		static readonly ParamInfo MethodInfo = new EnumInfo<SynthMethod>(
			nameof(Method), "Synthesis method", Methods);
		public Param Bpm { get; } = new(BpmInfo);
		public Param Method { get; } = new(MethodInfo);

		internal GlobalModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups()
		=> new[] { new[] { Bpm, Method } };
	}
}