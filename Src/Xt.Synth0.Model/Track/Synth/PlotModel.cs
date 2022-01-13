using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotFit { Auto, Rate, Fit }
	public enum PlotSource { Unit1, Unit2, Unit3, Env1, Env2 }

	public unsafe sealed class PlotModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int source, fit; }

		public Param Fit { get; } = new(FitInfo);
		public Param Source { get; } = new(SourceInfo);

		public string Name => "Plot";
		public ThemeGroup Group => ThemeGroup.Plot;
		public IReadOnlyList<Param> Params => new[] { Source, Fit };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;

		static readonly ParamInfo FitInfo = ParamInfo.List<PlotFit>(p => &((Native*)p)->fit, nameof(PlotFit), "Fit mode", true);
		static readonly ParamInfo SourceInfo = ParamInfo.List<PlotSource>(p => &((Native*)p)->source, nameof(PlotSource), "Graph source", true);
	}
}