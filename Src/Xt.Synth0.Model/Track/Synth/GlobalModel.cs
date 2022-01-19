using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int amp, env1; }

		public Param Amp { get; } = new(AmpInfo);
		public Param Env1 { get; } = new(Env1Info);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public IReadOnlyList<Param> Params => new[] { Amp, Env1 };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 0);
		static readonly ParamInfo Env1Info = ParamInfo.Level(p => &((Native*)p)->env1, "Env1->Amp", "Env1 to amp", true, 255);
	}
}