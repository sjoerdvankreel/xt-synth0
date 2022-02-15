using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum LfoType { Sin, Saw, Sqr, Tri }

    public unsafe sealed class LfoModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 32;
            internal int type;
            internal int on, sync, inv, bip;
            internal int rate, step, pad__;
        };

        public Param On { get; } = new(OnInfo);
        public Param Inv { get; } = new(InvInfo);
        public Param Bip { get; } = new(BipInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Sync { get; } = new(SyncInfo);
        public Param Rate { get; } = new(RateInfo);
        public Param Step { get; } = new(StepInfo);

        public int Columns => 3;
        public int Index { get; }
        public Param Enabled => On;
        public string[] In => null;
        public string Name => $"LFO {Index + 1}";
        public ThemeGroup ThemeGroup => ThemeGroup.Lfo;
        public string Id => "E2E5D904-8652-450B-A293-7CDFF05892BF";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public string[] Out => new[] { SynthModel.PartUnit, SynthModel.PartFilter, SynthModel.PartGlobal };
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->source.lfos[Index * Native.Size];
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Sync, 1 }, { Rate, 2 }, { Step, 2 },
            { Bip, 3 }, { Inv, 4 }
        };

        internal LfoModel(int index) => Index = index;
        static readonly IRelevance RelevanceSync = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 0);

        static readonly ParamInfo InvInfo = ParamInfo.Toggle(p => &((Native*)p)->inv, nameof(Inv), "Invert", "Invert", false);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), nameof(On), "Enabled", false);
        static readonly ParamInfo BipInfo = ParamInfo.Toggle(p => &((Native*)p)->bip, nameof(Bip), "Bipolar", "Bipolar", false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, nameof(Type), nameof(Type), "Type");
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, nameof(Sync), nameof(Sync), "Sync to beat", false);
        static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, nameof(Step), nameof(Step), "Rate steps", 1, 7, RelevanceSync);
        static readonly ParamInfo RateInfo = ParamInfo.Time(p => &((Native*)p)->rate, nameof(Rate), nameof(Rate), "Rate milliseconds", 1, 10, RelevanceTime);
    }
}