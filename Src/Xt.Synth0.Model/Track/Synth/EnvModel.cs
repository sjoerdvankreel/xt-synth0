using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum EnvType { DAHDSR, DAHDR }
    public enum SlopeType { Lin, Log, Inv, Sin, Cos }

    public unsafe sealed class EnvModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 72;
            internal int type;
            internal int on, sync, inv;
            internal int aSlp, dSlp, rSlp;
            internal int dly, a, hld, d, s, r;
            internal int dlyStp, aStp, hldStp, dStp, rStp;
        }

        public Param S { get; } = new(SInfo);
        public Param A { get; } = new(AInfo);
        public Param D { get; } = new(DInfo);
        public Param R { get; } = new(RInfo);
        public Param On { get; } = new(OnInfo);
        public Param Inv { get; } = new(InvInfo);
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

        public int Columns => 3;
        public int Index { get; }
        public Param Enabled => On;
        public string Name => $"Env {Index + 1}";
        public ThemeGroup ThemeGroup => ThemeGroup.Env;
        public string Id => "A7FF2DD9-62D5-4426-8530-02C60710237D";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->source.envs[Index * Native.Size];
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Sync, 1 }, { Inv, 2 },
            { Dly, 3 }, { DlyStp, 3 }, { A, 4 }, { AStp, 4 }, { ASlp, 5 },
            { Hld, 6 }, { HldStp, 6 }, { D, 7 }, { DStp, 7 }, { DSlp, 8 },
            { S, 9 }, { R, 10 }, { RStp, 10 }, { RSlp, 11 }
        };

        internal EnvModel(int index) => Index = index;
        static readonly IRelevance RelevanceSync = Relevance.Param((EnvModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.Param((EnvModel m) => m.Sync, (int s) => s == 0);

        static readonly ParamInfo SInfo = ParamInfo.Level(p => &((Native*)p)->s, 1, nameof(S), nameof(S), "Sustain level", 128);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), nameof(On), "Enabled", false);
        static readonly ParamInfo InvInfo = ParamInfo.Toggle(p => &((Native*)p)->inv, 0, nameof(Inv), "Invert", "Invert", false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<EnvType>(p => &((Native*)p)->type, 0, nameof(Type), nameof(Type), "Type");
        static readonly ParamInfo DSlpInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->dSlp, 2, nameof(DSlp), "Slp", "Decay slope");
        static readonly ParamInfo ASlpInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->aSlp, 2, nameof(ASlp), "Slp", "Attack slope");
        static readonly ParamInfo RSlpInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->rSlp, 2, nameof(RSlp), "Slp", "Release slope");
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 0, nameof(Sync), nameof(Sync), "Sync to beat", false);
        static readonly ParamInfo DStpInfo = ParamInfo.Step(p => &((Native*)p)->dStp, 1, nameof(DStp), "D", "Decay steps", 0, 11, RelevanceSync);
        static readonly ParamInfo AStpInfo = ParamInfo.Step(p => &((Native*)p)->aStp, 1, nameof(AStp), "A", "Attack steps", 0, 1, RelevanceSync);
        static readonly ParamInfo RStpInfo = ParamInfo.Step(p => &((Native*)p)->rStp, 1, nameof(RStp), "R", "Release steps", 0, 15, RelevanceSync);
        static readonly ParamInfo HldStpInfo = ParamInfo.Step(p => &((Native*)p)->hldStp, 1, nameof(HldStp), "Hld", "Hold steps", 0, 0, RelevanceSync);
        static readonly ParamInfo DlyStpInfo = ParamInfo.Step(p => &((Native*)p)->dlyStp, 1, nameof(DlyStp), "Dly", "Delay steps", 0, 0, RelevanceSync);
        static readonly ParamInfo DInfo = ParamInfo.Time(p => &((Native*)p)->d, 1, nameof(D), nameof(D), "Decay milliseconds", 0, 18, RelevanceTime);
        static readonly ParamInfo AInfo = ParamInfo.Time(p => &((Native*)p)->a, 1, nameof(A), nameof(A), "Attack milliseconds", 0, 7, RelevanceTime);
        static readonly ParamInfo RInfo = ParamInfo.Time(p => &((Native*)p)->r, 1, nameof(R), nameof(R), "Release milliseconds", 0, 36, RelevanceTime);
        static readonly ParamInfo HldInfo = ParamInfo.Time(p => &((Native*)p)->hld, 1, nameof(Hld), nameof(Hld), "Hold milliseconds", 0, 0, RelevanceTime);
        static readonly ParamInfo DlyInfo = ParamInfo.Time(p => &((Native*)p)->dly, 1, nameof(Dly), nameof(Dly), "Delay milliseconds", 0, 0, RelevanceTime);
    }
}