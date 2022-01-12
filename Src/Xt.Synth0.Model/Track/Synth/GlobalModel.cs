using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotType { Unit1, Unit2, Unit3, Env1, Env2 }

	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int bpm, amp, fitPlot, plot; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Plot { get; } = new(PlotInfo);
		public Param FitPlot { get; } = new(FitPlotInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.GlobalPlot;
		public IReadOnlyList<Param> Params => new[] { Bpm, Amp, FitPlot, Plot };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo BpmInfo = ParamInfo.Lin(p => &((Native*)p)->bpm, "BPM", 1, 255, 120);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 128);
		static readonly ParamInfo PlotInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->plot, nameof(Plot));
		static readonly ParamInfo FitPlotInfo = ParamInfo.Toggle(p => &((Native*)p)->fitPlot, nameof(FitPlot), false);
	}
}