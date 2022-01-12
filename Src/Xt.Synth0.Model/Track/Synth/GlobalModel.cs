using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotFit { Auto, Rate, Fit }
	public enum PlotSource { Unit1, Unit2, Unit3, Env1, Env2 }

	public unsafe sealed class GlobalModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int bpm, amp, plotFit, plotSource; }

		public Param Bpm { get; } = new(BpmInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param PlotFit { get; } = new(PlotFitInfo);
		public Param PlotSource { get; } = new(PlotSourceInfo);

		public string Name => "Global";
		public ThemeGroup Group => ThemeGroup.GlobalPlot;
		public IReadOnlyList<Param> Params => new[] { Bpm, Amp, PlotFit, PlotSource };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;

		static readonly ParamInfo BpmInfo = ParamInfo.Lin(p => &((Native*)p)->bpm, "BPM", 1, 255, 120);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 128);
		static readonly ParamInfo PlotFitInfo = ParamInfo.List<PlotFit>(p => &((Native*)p)->plotFit, nameof(PlotSource));
		static readonly ParamInfo PlotSourceInfo = ParamInfo.List<PlotSource>(p => &((Native*)p)->plotSource, nameof(PlotSource));
	}
}