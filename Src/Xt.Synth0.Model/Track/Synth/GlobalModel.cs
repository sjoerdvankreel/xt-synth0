using System;
using System.Linq;

namespace Xt.Synth0.Model
{
	public sealed class GlobalModel : GroupModel
	{
		internal struct Native
		{
			internal int bpm;
			internal int hmns;
			internal int plot;
			internal int method;
		}

		public static readonly string[] Methods = Enum.
			GetValues<SynthMethod>().Select(v => v.ToString()).ToArray();

		static readonly ParamInfo BpmInfo = new DiscreteInfo(
			nameof(Bpm), "Tempo", 1, 255, 120);
		static readonly ParamInfo PlotInfo = new DiscreteInfo(
			nameof(Plot), "Plot unit", 1, SynthModel.UnitCount, 1);
		static readonly ParamInfo HmnsInfo = new ExpInfo(
			nameof(Hmns), "Additive harmonics", 0, 10, 4);
		static readonly ParamInfo MethodInfo = new EnumInfo<SynthMethod>(
			nameof(Method), "Method (PolyBLEP, Additive, Naive)", Methods);

		public Param Bpm { get; } = new(BpmInfo);
		public Param Hmns { get; } = new(HmnsInfo);
		public Param Plot { get; } = new(PlotInfo);
		public Param Method { get; } = new(MethodInfo);

		internal GlobalModel(string name) : base(name) { }
		internal override Param[][] ListParamGroups() => new[]
		{
			new[] { Bpm, Plot },
			new[] { Method, Hmns }
		};

		internal void ToNative(ref Native native)
		{
			native.bpm = Bpm.Value;
			native.hmns = Hmns.Value;
			native.plot = Plot.Value;
			native.method = Method.Value;
		}

		internal void FromNative(ref Native native)
		{
			Bpm.Value = native.bpm;
			Hmns.Value = native.hmns;
			Plot.Value = native.plot;
			Method.Value = native.method;
		}
	}
}