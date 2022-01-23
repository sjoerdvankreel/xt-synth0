using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum GlobalAmpEnv { Env1, Env2, Env3 }

	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int ampEnv, amp, ampEnvAmt, pad__; }

		public Param Amp { get; } = new(AmpInfo);
		public Param AmpEnv { get; } = new(AmpEnvInfo);
		public Param AmpEnvAmt { get; } = new(AmpEnvAmtInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public IReadOnlyList<Param> Params => new[] { Amp, AmpEnv, AmpEnvAmt };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 0);
		static readonly ParamInfo AmpEnvAmtInfo = ParamInfo.Level(p => &((Native*)p)->ampEnvAmt, nameof(AmpEnvAmt), "Amp env amount", true, 255);
		static readonly ParamInfo AmpEnvInfo = ParamInfo.List<GlobalAmpEnv>(p => &((Native*)p)->ampEnv, nameof(AmpEnv), "Amp env source", false);
	}
}