using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotType { SynthL, SynthR, Unit1, Unit2, Unit3, Env1, Env2 }

	public unsafe sealed class PlotModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native { internal int type, pad__; }

		public string Name => "Plot";
		public ThemeGroup Group => ThemeGroup.Plot;
		public IReadOnlyList<Param> Params => new[] { Type };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;

		public Param Type { get; } = new(TypeInfo);
		static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, nameof(Type), "Source", false);
	}
}