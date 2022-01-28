using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PlotType { Off, Unit1, Unit2, Unit3, Env1, Env2, Env3, LFO1, LFO2, SynthL, SynthR }

	public unsafe sealed class PlotModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native { internal int type, hold; }

		public Param Type { get; } = new(TypeInfo);
		public Param Hold { get; } = new(HoldInfo);

		public int Columns => 3;
		public Param Enabled => null;
		public string Name => "Plot";
		public ThemeGroup ThemeGroup => ThemeGroup.Plot;
		public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;
		public IDictionary<Param, int> Layout => new Dictionary<Param, int>() { { Type, 0 }, { Hold, 1 } };

		static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, nameof(Type), "Source");
		static readonly ParamInfo HoldInfo = ParamInfo.Time(p => &((Native*)p)->hold, nameof(Hold), "Hold key time", 1, 10);
	}
}