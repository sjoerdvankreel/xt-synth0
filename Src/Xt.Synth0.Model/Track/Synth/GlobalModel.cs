using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int bpm, amp, ampSrc, ampAmt; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param AmpSrc { get; } = new(AmpSrcInfo);
		public Param AmpAmt { get; } = new(AmpAmtInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public IReadOnlyList<Param> Params => new[] { Bpm, Amp, AmpSrc, AmpAmt };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 128);
		static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, "BPM", "Beats per minute", true, 1, 255, 120);
		static readonly ParamInfo AmpAmtInfo = ParamInfo.Level(p => &((Native*)p)->ampAmt, "Amt", "Envelope to amp amount", true, 255);
		static readonly ParamInfo AmpSrcInfo = ParamInfo.List<AmpEnvSource>(p => &((Native*)p)->ampSrc, nameof(AmpSrc), "Amp envelope source", true);
	}
}