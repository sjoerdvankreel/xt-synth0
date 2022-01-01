using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native { internal int bpm, hmns, plot, method; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Hmns { get; } = new(HmnsInfo);
		public Param Plot { get; } = new(PlotInfo);
		public Param Method { get; } = new(MethodInfo);

		public string Name => "Global";
		public IReadOnlyList<Param> Params => new[] { Bpm, Hmns, Plot, Method };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly string[] Methods = Enum.GetValues<SynthMethod>().Select(v => v.ToString()).ToArray();
		static readonly ParamInfo BpmInfo = new DiscreteInfo(p => &((Native*)p)->bpm, nameof(Bpm), "Tempo", 1, 255, 120);
		static readonly ParamInfo HmnsInfo = new ExpInfo(p => &((Native*)p)->hmns, nameof(Hmns), "Additive harmonics", 0, 10, 4);
		static readonly ParamInfo PlotInfo = new DiscreteInfo(p => &((Native*)p)->plot, nameof(Plot), "Plot unit", 1, TrackConstants.UnitCount, 1);
		static readonly ParamInfo MethodInfo = new EnumInfo<SynthMethod>(p => &((Native*)p)->method, nameof(Method), "Method (PolyBLEP, Additive, Naive)", Methods);
	}
}