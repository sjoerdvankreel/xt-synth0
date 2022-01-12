using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int bpm, amp; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.GlobalPlot;
		public IReadOnlyList<Param> Params => new[] { Bpm, Amp };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo BpmInfo = ParamInfo.Lin(p => &((Native*)p)->bpm, "BPM", 1, 255, 120);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 128);
	}
}