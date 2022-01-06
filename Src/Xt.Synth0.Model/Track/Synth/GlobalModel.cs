using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int bpm, amp, plot, pad__; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Plot { get; } = new(PlotInfo);

		public string Name => "Global";
		public IReadOnlyList<Param> Params => new[] { Bpm, Amp, Plot };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo BpmInfo = ParamInfo.Lin(p => &((Native*)p)->bpm, nameof(Bpm), 1, 255, 120);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 128);
		static readonly ParamInfo PlotInfo = ParamInfo.Lin(p => &((Native*)p)->plot, nameof(Plot), 1, TrackConstants.UnitCount, 1);
	}
}