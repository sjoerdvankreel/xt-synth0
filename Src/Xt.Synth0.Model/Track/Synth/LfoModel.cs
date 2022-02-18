using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum LfoType { Sin, Saw, Sqr, Tri }
    public enum LfoPolarity {  Uni, UniInv, Bi, BiInv }

    public unsafe sealed class LfoModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 24;
            internal int type;
            internal int plty;
            internal int on, sync;
            internal int rate, step;
        };

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Plty { get; } = new(PltyInfo);
        public Param Sync { get; } = new(SyncInfo);
        public Param Rate { get; } = new(RateInfo);
        public Param Step { get; } = new(StepInfo);

        public int Columns => 4;
        public int Index { get; }
        public Param Enabled => On;
        public string Name => $"LFO {Index + 1}";
        public ThemeGroup ThemeGroup => ThemeGroup.Lfo;
        public string Id => "E2E5D904-8652-450B-A293-7CDFF05892BF";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->source.lfos[Index * Native.Size];
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Plty, 1 }, { Sync, 2 }, { Rate, 3 }, { Step, 3 }
        };

        internal LfoModel(int index) => Index = index;
        static readonly IRelevance RelevanceSync = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.When((LfoModel m) => m.Sync, (int s) => s == 0);

        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), nameof(On), "Enabled", false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, 0, nameof(Type), nameof(Type), "Type");
        static readonly ParamInfo PltyInfo = ParamInfo.List<LfoPolarity>(p => &((Native*)p)->plty, 0, nameof(Plty), "Polarity", "Polarity");
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 1, nameof(Sync), nameof(Sync), "Sync to beat", false);
        static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, 1, nameof(Step), nameof(Step), "Rate steps", 1, 7, RelevanceSync);
        static readonly ParamInfo RateInfo = ParamInfo.Time(p => &((Native*)p)->rate, 1, nameof(Rate), nameof(Rate), "Rate milliseconds", 1, 26, RelevanceTime);
    }
}