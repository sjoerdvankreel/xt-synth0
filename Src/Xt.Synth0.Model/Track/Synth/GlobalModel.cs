using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int bpm, lvl, plot, pad__; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Lvl { get; } = new(LvlInfo);
		public Param Plot { get; } = new(PlotInfo);

		public string Name => "Global";
		public IReadOnlyList<Param> Params => new[] { Bpm, Lvl, Plot };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo LvlInfo = new ContinuousInfo(p => &((Native*)p)->lvl, nameof(Lvl), "Volume", 128);
		static readonly ParamInfo BpmInfo = new DiscreteInfo(p => &((Native*)p)->bpm, nameof(Bpm), "Tempo", 1, 255, 120);
		static readonly ParamInfo PlotInfo = new DiscreteInfo(p => &((Native*)p)->plot, nameof(Plot), "Plot unit", 1, TrackConstants.UnitCount, 1);
	}
}