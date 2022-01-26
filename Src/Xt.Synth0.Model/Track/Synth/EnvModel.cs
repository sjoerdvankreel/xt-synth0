using MessagePack;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum EnvType { DAHDSR, DAHDR}

	public unsafe sealed class EnvModel : IThemedSubModel, IStoredModel<EnvModel.Native, EnvModel.Native>
	{
		[MessagePackObject(keyAsPropertyName: true)]
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		public struct Native
		{
			public const int Size = 72;
			public int on, type, sync;
			public int aSlp, dSlp, rSlp;
			public int dly, a, hld, d, s, r;
			public int dlyStp, aStp, hldStp, dStp, rStp, pad__;
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
		public Param AStp { get; } = new(AStpInfo);
		public Param DStp { get; } = new(DStpInfo);
		public Param RStp { get; } = new(RStpInfo);
		public Param HldStp { get; } = new(HldStpInfo);
		public Param DlyStp { get; } = new(DlyStpInfo);

		readonly int _index;
		public string Name => $"Env {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Env;
		internal EnvModel(int index) => _index = index;
		public void Load(ref Native stored, ref Native native) => native = stored;
		public void Store(ref Native native, ref Native stored) => stored = native;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->envs[_index * Native.Size];

		public Param Enabled => On;
		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { Sync, 1 }, { Dly, 2 }, { DlyStp, 2 },
			{ ASlp, 3 }, { A, 4 }, { AStp, 4 }, { Hld, 5 }, { HldStp, 5 },
			{ DSlp, 6 }, { D, 7 }, { DStp, 7 }, { S, 8 },
			{ RSlp, 9 }, { R, 10 }, { RStp, 10 }
		};

		static readonly IRelevance RelevanceSync = Relevance.When((EnvModel m) => m.Sync, (int s) => s == 1);
		static readonly IRelevance RelevanceTime = Relevance.When((EnvModel m) => m.Sync, (int s) => s == 0);

		static readonly ParamInfo DSlpInfo = ParamInfo.Mix(p => &((Native*)p)->dSlp, "Slp", "Decay slope");
		static readonly ParamInfo ASlpInfo = ParamInfo.Mix(p => &((Native*)p)->aSlp, "Slp", "Attack slope");
		static readonly ParamInfo RSlpInfo = ParamInfo.Mix(p => &((Native*)p)->rSlp, "Slp", "Release slope");
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), "Enabled", false);
		static readonly ParamInfo TypeInfo = ParamInfo.List<EnvType>(p => &((Native*)p)->type, nameof(Type), "Type");
		static readonly ParamInfo SInfo = ParamInfo.Level(p => &((Native*)p)->s, nameof(S), "Sustain level", 128, null);
		static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, nameof(Sync), "Sync to beat", false);
		static readonly ParamInfo DStpInfo = ParamInfo.Step(p => &((Native*)p)->dStp, "D", "Decay steps", 0, 11, RelevanceSync);
		static readonly ParamInfo AStpInfo = ParamInfo.Step(p => &((Native*)p)->aStp, "A", "Attack steps", 0, 1, RelevanceSync);
		static readonly ParamInfo RStpInfo = ParamInfo.Step(p => &((Native*)p)->rStp, "R", "Release steps", 0, 15, RelevanceSync);
		static readonly ParamInfo HldStpInfo = ParamInfo.Step(p => &((Native*)p)->hldStp, "Hld", "Hold steps", 0, 0, RelevanceSync);
		static readonly ParamInfo DlyStpInfo = ParamInfo.Step(p => &((Native*)p)->dlyStp, "Dly", "Delay steps", 0, 0, RelevanceSync);
		static readonly ParamInfo DInfo = ParamInfo.Time(p => &((Native*)p)->d, nameof(D), "Decay milliseconds", 0, 7, RelevanceTime);
		static readonly ParamInfo AInfo = ParamInfo.Time(p => &((Native*)p)->a, nameof(A), "Attack milliseconds", 0, 3, RelevanceTime);
		static readonly ParamInfo RInfo = ParamInfo.Time(p => &((Native*)p)->r, nameof(R), "Release milliseconds", 0, 14, RelevanceTime);
		static readonly ParamInfo HldInfo = ParamInfo.Time(p => &((Native*)p)->hld, nameof(Hld), "Hold milliseconds", 0, 0, RelevanceTime);
		static readonly ParamInfo DlyInfo = ParamInfo.Time(p => &((Native*)p)->dly, nameof(Dly), "Delay milliseconds", 0, 0, RelevanceTime);
	}
}