using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum EnvType { Off, DAHDR, DAHDSR }

	public unsafe sealed class EnvModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int a, d, s, r, hld, dly;
			internal int type, aSlp, dSlp, rSlp;
		}

		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param S { get; } = new(SInfo);
		public Param R { get; } = new(RInfo);
		public Param Hld { get; } = new(HldInfo);
		public Param Dly { get; } = new(DlyInfo);
		public Param ASlp { get; } = new(ASlpInfo);
		public Param DSlp { get; } = new(DSlpInfo);
		public Param RSlp { get; } = new(RSlpInfo);
		public Param Type { get; } = new(TypeInfo);

		readonly int _index;
		public int ColumnCount => 3;
		public string Name => $"Env {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Envs;
		internal EnvModel(int index) => _index = index;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->envs[_index * TrackConstants.EnvModelSize];

		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ Type, 0 },
			{ Dly, 2 },
			{ ASlp, 3 },
			{ A, 4 },
			{ Hld, 5 },
			{ DSlp, 6 },
			{ D, 7 },
			{ S, 8 },
			{ RSlp, 9 },
			{ R, 10 }
		};

		static readonly IRelevance RelevanceDAHDSR = Relevance.When((EnvModel m) => m.Type, (EnvType t) => t == EnvType.DAHDSR);

		static readonly ParamInfo DSlpInfo = ParamInfo.Mix(p => &((Native*)p)->dSlp, "Slp", "Decay slope", true);
		static readonly ParamInfo ASlpInfo = ParamInfo.Mix(p => &((Native*)p)->aSlp, "Slp", "Attack slope", true);
		static readonly ParamInfo RSlpInfo = ParamInfo.Mix(p => &((Native*)p)->rSlp, "Slp", "Release slope", true);
		static readonly ParamInfo TypeInfo = ParamInfo.List<EnvType>(p => &((Native*)p)->type, nameof(Type), "Type", false);
		static readonly ParamInfo DInfo = ParamInfo.Time(p => &((Native*)p)->d, nameof(D), "Decay milliseconds", true, 7);
		static readonly ParamInfo AInfo = ParamInfo.Time(p => &((Native*)p)->a, nameof(A), "Attack milliseconds", true, 3);
		static readonly ParamInfo RInfo = ParamInfo.Time(p => &((Native*)p)->r, nameof(R), "Release milliseconds", true, 14);
		static readonly ParamInfo HldInfo = ParamInfo.Time(p => &((Native*)p)->hld, nameof(Hld), "Hold milliseconds", true, 0);
		static readonly ParamInfo DlyInfo = ParamInfo.Time(p => &((Native*)p)->dly, nameof(Dly), "Delay milliseconds", true, 0);
		static readonly ParamInfo SInfo = ParamInfo.Level(p => &((Native*)p)->s, nameof(S), "Sustain level", true, 128, null, RelevanceDAHDSR);
	}
}