using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotHold { Hold, Touch, Hold1Ms, Hold10Ms, Hold100Ms, Hold1S, Hold10S }
	public enum PlotType { Off, SynthL, SynthR, Unit1, Unit2, Unit3, Env1, Env2, Env3, LFO1, LFO2 }

	public unsafe sealed class PlotModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int type, hold; }

		public Param Type { get; } = new(TypeInfo);
		public Param Hold { get; } = new(HoldInfo);

		public string Name => "Plot";
		public ThemeGroup Group => ThemeGroup.Plot;
		public IReadOnlyList<Param> Params => new[] { Type, Hold };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;

		static readonly ParamInfo HoldInfo = ParamInfo.List<PlotHold>(p => &((Native*)p)->hold, nameof(Hold), "Hold");
		static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, nameof(Type), "Source");
	}
}