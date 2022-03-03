using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum LfoType { Sin, Saw, Sqr, Tri }
    public enum LfoPolarity { Unipolar, UnipolarInv, Bipolar, BipolarInv }

    public unsafe sealed class LfoModel : IUIParamGroupModel
    {
        const double MinFreqHz = 0.1;
        const double MaxFreqHz = 20.0;

        public int Index { get; }
        internal LfoModel(int index) => Index = index;

        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Lfo;

        public string Name => $"LFO {Index + 1}";
        public string Id => "E2E5D904-8652-450B-A293-7CDFF05892BF";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->cv.lfos[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Polarity, 1 }, { Sync, 2 }, { Frequency, 3 }, { Step, 3 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 24;

            internal int on;
            internal int sync;
            internal int type;
            internal int step;
            internal int frequency;
            internal int polarity;
        }

        static readonly IRelevance RelevanceSync = Relevance.Param((LfoModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.Param((LfoModel m) => m.Sync, (int s) => s == 0);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Polarity { get; } = new(PolarityInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, 2, nameof(Type), "Type", "Type");
        static readonly ParamInfo PolarityInfo = ParamInfo.List<LfoPolarity>(p => &((Native*)p)->polarity, 2, nameof(Polarity), "Polarity", "Polarity");

        public Param Sync { get; } = new(SyncInfo);
        public Param Step { get; } = new(StepInfo);
        public Param Frequency { get; } = new(FrequencyInfo);
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 1, nameof(Sync), "Sync", "Sync to beat", false);
        static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, 1, nameof(Step), "Step", "Rate steps", 1, 7, RelevanceSync);
        static readonly ParamInfo FrequencyInfo = ParamInfo.Frequency(p => &((Native*)p)->frequency, 1, nameof(Frequency), "Frq", "Frequency", 0, MinFreqHz, MaxFreqHz, RelevanceTime);
    }
}