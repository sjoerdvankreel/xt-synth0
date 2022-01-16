using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int env, amp, bpm, pad__; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param AmpEnv { get; } = new(AmpEnvInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public IReadOnlyList<Param> Params => new[] { AmpEnv, Amp, Bpm };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 128);
		static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, "BPM", "Beats per minute", true, 1, 255, 120);
		static readonly ParamInfo AmpEnvInfo = ParamInfo.List<AmpEnv>(p => &((Native*)p)->env, nameof(AmpEnv), "Amp envelope", true);
	}
}