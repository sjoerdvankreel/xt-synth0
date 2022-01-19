using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotFit { Auto, Rate, Fit }
	public enum PlotSource { Global, Unit1, Unit2, Unit3, Env1, Env2 }

	public unsafe sealed class PlotModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int fit, src; }

		public Param Fit { get; } = new(FitInfo);
		public Param Src { get; } = new(SrcInfo);

		public string Name => "Plot";
		public ThemeGroup Group => ThemeGroup.Plot;
		public IReadOnlyList<Param> Params => new[] { Src, Fit };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;

		static readonly ParamInfo FitInfo = ParamInfo.List<PlotFit>(p => &((Native*)p)->fit, nameof(PlotFit), "Fit mode", false);
		static readonly ParamInfo SrcInfo = ParamInfo.List<PlotSource>(p => &((Native*)p)->src, nameof(PlotSource), "Graph source", false);
	}
}