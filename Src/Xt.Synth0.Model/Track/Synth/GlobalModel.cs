using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum AmpSource { Env1, Env2, Env3 }

	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int src, amp, amt, pad__; }

		public Param Amp { get; } = new(AmpInfo);
		public Param Src { get; } = new(SrcInfo);
		public Param Amt { get; } = new(AmtInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public IReadOnlyList<Param> Params => new[] { Amp, Src, Amt };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 0);
		static readonly ParamInfo AmtInfo = ParamInfo.Level(p => &((Native*)p)->amt, nameof(Amt), "Amp env amount", true, 255);
		static readonly ParamInfo SrcInfo = ParamInfo.List<AmpSource>(p => &((Native*)p)->src, "Source", "Amp env source", false);
	}
}