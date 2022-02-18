using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotType { Off, Amp, Env1, Env2, Env3, LFO1, LFO2, LFO3, Unit1, Unit2, Unit3, SynthL, SynthR }

	public unsafe sealed class PlotModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native { internal int spec, type, hold, pad__; }

		public Param Spec { get; } = new(SpecInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Hold { get; } = new(HoldInfo);

		public int Index => 0;
		public int Columns => 3;
		public Param Enabled => null;
		public string Name => "Plot";
		public ThemeGroup ThemeGroup => ThemeGroup.Plot;
		public string Id => "BD224A37-6B8E-4EDA-9E49-DE3DD1AF61CE";
		public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;
		public IDictionary<Param, int> Layout => new Dictionary<Param, int>() { { Type, 0 }, { Spec, 1 }, { Hold, 2 } };

		static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, 1, nameof(Type), nameof(Type), "Source");
		static readonly ParamInfo SpecInfo = ParamInfo.Toggle(p => &((Native*)p)->spec, 1, nameof(Spec), nameof(Spec), "Spectrum", false);
		static readonly ParamInfo HoldInfo = ParamInfo.Time(p => &((Native*)p)->hold, 1, nameof(Hold), nameof(Hold), "Hold key time", 1, 26);
	}
}