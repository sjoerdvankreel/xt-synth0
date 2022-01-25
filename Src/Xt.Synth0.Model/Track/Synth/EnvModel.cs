using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum EnvType { DAHDSR, DAHDR}

	public unsafe sealed class EnvModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native
		{
			internal const int Size = 72;
			internal int on, type, sync;
			internal int aSlp, dSlp, rSlp;
			internal int dly, a, hld, d, s, r;
			internal int dlySnc, aSnc, hldSnc, dSnc, rSnc, pad__;
		}

		public Param S { get; } = new(SInfo);
		public Param A { get; } = new(AInfo);
		public Param D { get; } = new(DInfo);
		public Param R { get; } = new(RInfo);
		public Param On { get; } = new(OnInfo);
		public Param Hld { get; } = new(HldInfo);
		public Param Dly { get; } = new(DlyInfo);
		public Param ASlp { get; } = new(ASlpInfo);
		public Param DSlp { get; } = new(DSlpInfo);
		public Param RSlp { get; } = new(RSlpInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Sync { get; } = new(SyncInfo);
		public Param ASnc { get; } = new(ASncInfo);
		public Param DSnc { get; } = new(DSncInfo);
		public Param RSnc { get; } = new(RSncInfo);
		public Param HldSnc { get; } = new(HldSncInfo);
		public Param DlySnc { get; } = new(DlySncInfo);

		readonly int _index;
		public string Name => $"Env {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Env;
		internal EnvModel(int index) => _index = index;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->envs[_index * Native.Size];

		public Param Enabled => On;
		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { Sync, 1 }, { Dly, 2 }, { DlySnc, 2 },
			{ ASlp, 3 }, { A, 4 }, { ASnc, 4 }, { Hld, 5 }, { HldSnc, 5 },
			{ DSlp, 6 }, { D, 7 }, { DSnc, 7 }, { S, 8 },
			{ RSlp, 9 }, { R, 10 }, { RSnc, 10 }
		};

		static readonly IRelevance RelevanceSync = Relevance.When((EnvModel m) => m.Sync, (int s) => s == 1);
		static readonly IRelevance RelevanceTime = Relevance.When((EnvModel m) => m.Sync, (int s) => s == 0);
		static readonly IRelevance RelevanceDAHDSR = Relevance.When((EnvModel m) => m.Type, (EnvType t) => t == EnvType.DAHDSR);

		static readonly ParamInfo DSlpInfo = ParamInfo.Mix(p => &((Native*)p)->dSlp, "Slp", "Decay slope");
		static readonly ParamInfo ASlpInfo = ParamInfo.Mix(p => &((Native*)p)->aSlp, "Slp", "Attack slope");
		static readonly ParamInfo RSlpInfo = ParamInfo.Mix(p => &((Native*)p)->rSlp, "Slp", "Release slope");
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), "Enabled", false);
		static readonly ParamInfo TypeInfo = ParamInfo.List<EnvType>(p => &((Native*)p)->type, nameof(Type), "Type");
		static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, nameof(Sync), "Sync to beat", false);
		static readonly ParamInfo DInfo = ParamInfo.Time(p => &((Native*)p)->d, nameof(D), "Decay milliseconds", 0, 7, RelevanceTime);
		static readonly ParamInfo AInfo = ParamInfo.Time(p => &((Native*)p)->a, nameof(A), "Attack milliseconds", 0, 3, RelevanceTime);
		static readonly ParamInfo RInfo = ParamInfo.Time(p => &((Native*)p)->r, nameof(R), "Release milliseconds", 0, 14, RelevanceTime);
		static readonly ParamInfo HldInfo = ParamInfo.Time(p => &((Native*)p)->hld, nameof(Hld), "Hold milliseconds", 0, 0, RelevanceTime);
		static readonly ParamInfo DlyInfo = ParamInfo.Time(p => &((Native*)p)->dly, nameof(Dly), "Delay milliseconds", 0, 0, RelevanceTime);
		static readonly ParamInfo SInfo = ParamInfo.Level(p => &((Native*)p)->s, nameof(S), "Sustain level", 128, null, RelevanceDAHDSR);
		static readonly ParamInfo DSncInfo = ParamInfo.Select(p => &((Native*)p)->dSnc, "D", "Decay steps", SyncStep.S0, SyncStep.S1_4, SynthModel.SyncStepNames, RelevanceSync);
		static readonly ParamInfo ASncInfo = ParamInfo.Select(p => &((Native*)p)->aSnc, "A", "Attack steps", SyncStep.S0, SyncStep.S1_16, SynthModel.SyncStepNames, RelevanceSync);
		static readonly ParamInfo RSncInfo = ParamInfo.Select(p => &((Native*)p)->rSnc, "R", "Release steps", SyncStep.S0, SyncStep.S1_1, SynthModel.SyncStepNames, RelevanceSync);
		static readonly ParamInfo HldSncInfo = ParamInfo.Select(p => &((Native*)p)->hldSnc, "Hld", "Hold steps", SyncStep.S0, SyncStep.S0, SynthModel.SyncStepNames, RelevanceSync);
		static readonly ParamInfo DlySncInfo = ParamInfo.Select(p => &((Native*)p)->dlySnc, "Dly", "Delay steps", SyncStep.S0, SyncStep.S0, SynthModel.SyncStepNames, RelevanceSync);
	}
}