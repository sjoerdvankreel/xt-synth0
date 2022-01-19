using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int bpm, env1; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Env1 { get; } = new(Env1Info);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.Global;
		public IReadOnlyList<Param> Params => new[] { Bpm, Env1 };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo Env1Info = ParamInfo.Level(p => &((Native*)p)->env1, nameof(Env1), "Env1 to amp", true, 128);
		static readonly ParamInfo BpmInfo = ParamInfo.Select(p => &((Native*)p)->bpm, "BPM", "Beats per minute", true, 1, 255, 120);
	}
}