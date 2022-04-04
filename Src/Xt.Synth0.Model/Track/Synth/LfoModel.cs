using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum LfoShape { Bi, Uni, BiInv, UniInv };
    public enum LfoType { Sin, Saw, Sqr, Tri, Rnd };

    public unsafe sealed class LfoModel : IUIParamGroupModel
    {
        const double MinFreqHz = 0.1;
        const double MaxFreqHz = 20.0;

        readonly bool _global;
        public int Index { get; }
        internal LfoModel(bool global, int index) => (_global, Index) = (global, index);

        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => _global ? ThemeGroup.GlobalLfo : ThemeGroup.VoiceLfo;

        public string Name => $"LFO {Index + 1}";
        public string Info => _global ? "Global" : null;
        public string Id => "E2E5D904-8652-450B-A293-7CDFF05892BF";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => _global ? &((SynthModel.Native*)parent)->global.lfo : &((SynthModel.Native*)parent)->voice.cv.lfos[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Shape, 1 }, { Smooth, 2 }, { Sync, 3 },
            { RandomSeed, 4 }, { RandomNext, 5 }, { RandomSteepness, 6 }, { Frequency, 7 }, { Step, 7 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 40;

            internal int on;
            internal int type;
            internal int sync;
            internal int shape;
            internal int step;
            internal int smooth;
            internal int frequency;
            internal int randomSeed;
            internal int randomNext;
            internal int randomSteepness;
        }

        static readonly IRelevance RelevanceSync = Relevance.Param((LfoModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.Param((LfoModel m) => m.Sync, (int s) => s == 0);
        static readonly IRelevance RelevanceRandom = Relevance.Param((LfoModel m) => m.Type, (LfoType t) => t == LfoType.Rnd);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Smooth = new Param(SmoothInfo);
        public Param Shape { get; } = new(ShapeInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", true, false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, 0, nameof(Type), "Type", "Type", true);
        static readonly ParamInfo ShapeInfo = ParamInfo.List<LfoShape>(p => &((Native*)p)->shape, 0, nameof(Shape), "Shape", "Shape", true);
        static readonly ParamInfo SmoothInfo = ParamInfo.Level(p => &((Native*)p)->smooth, 0, nameof(Smooth), "Smt", "Smoothing amount", true, 0);

        public Param RandomSeed = new Param(RandomSeedInfo);
        public Param RandomNext = new Param(RandomNextInfo);
        public Param RandomSteepness = new Param(RandomSteepnessInfo);
        static readonly ParamInfo RandomSeedInfo = ParamInfo.Level(p => &((Native*)p)->randomSeed, 2, nameof(RandomSeed), "Sed", "Random seed", true, 0, RelevanceRandom);
        static readonly ParamInfo RandomNextInfo = ParamInfo.Level(p => &((Native*)p)->randomNext, 2, nameof(RandomNext), "Nxt", "Random next", true, 0, RelevanceRandom);
        static readonly ParamInfo RandomSteepnessInfo = ParamInfo.Level(p => &((Native*)p)->randomSteepness, 2, nameof(RandomSteepness), "Stn", "Random steepness", true, 0, RelevanceRandom);

        public Param Sync { get; } = new(SyncInfo);
        public Param Step { get; } = new(StepInfo);
        public Param Frequency { get; } = new(FrequencyInfo);
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 1, nameof(Sync), "Sync", "Beat sync", true, false);
        static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, 1, nameof(Step), "Step", "Rate steps", true, 1, 7, RelevanceSync);
        static readonly ParamInfo FrequencyInfo = ParamInfo.Frequency(p => &((Native*)p)->frequency, 1, nameof(Frequency), "Frq", "Frequency", true, 0, MinFreqHz, MaxFreqHz, RelevanceTime);
    }
}