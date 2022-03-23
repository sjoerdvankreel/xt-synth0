using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum LfoType { Sin, Saw, Sqr, Tri }

    public unsafe sealed class LfoModel : IAutomationGroupModel
    {
        const double MinFreqHz = 0.1;
        const double MaxFreqHz = 20.0;

        readonly bool _global;
        public int Index { get; }
        internal LfoModel(bool global, int index) => (_global, Index) = (global, index);

        public int Columns => 5;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Lfo;
        public int AutomationId => SynthConfig.SynthAutomationVoiceLfo1 + Index;

        public string Name => $"LFO {Index + 1}";
        public string Info => _global ? "Global" : null;
        public string Id => "E2E5D904-8652-450B-A293-7CDFF05892BF";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => _global ? &((SynthModel.Native*)parent)->global.lfo : &((SynthModel.Native*)parent)->voice.cv.lfos[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Unipolar, 1 }, { Invert, 2 }, { Sync, 3 }, { Frequency, 4 }, { Step, 4 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 32;

            internal int on;
            internal int type;
            internal int sync;
            internal int invert;
            internal int unipolar;
            internal int step;
            internal int frequency;
            internal int pad__;
        }

        static readonly IRelevance RelevanceSync = Relevance.Param((LfoModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.Param((LfoModel m) => m.Sync, (int s) => s == 0);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Invert { get; } = new(InvertInfo);
        public Param Unipolar { get; } = new Param(UnipolarInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", false, false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<LfoType>(p => &((Native*)p)->type, 2, nameof(Type), "Type", "Type", false);
        static readonly ParamInfo InvertInfo = ParamInfo.Toggle(p => &((Native*)p)->invert, 0, nameof(Invert), "Inv", "Invert", false, false);
        static readonly ParamInfo UnipolarInfo = ParamInfo.Toggle(p => &((Native*)p)->unipolar, 0, nameof(Unipolar), "Uni", "Unipolar", false, false);

        public Param Sync { get; } = new(SyncInfo);
        public Param Step { get; } = new(StepInfo);
        public Param Frequency { get; } = new(FrequencyInfo);
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 0, nameof(Sync), "Sync", "Beat sync", false, false);
        static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, 1, nameof(Step), "Step", "Rate steps", false, 1, 7, RelevanceSync);
        static readonly ParamInfo FrequencyInfo = ParamInfo.Frequency(p => &((Native*)p)->frequency, 1, nameof(Frequency), "Frq", "Frequency", false, 0, MinFreqHz, MaxFreqHz, RelevanceTime);
    }
}