using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class DelayModel : IUIParamGroupModel
    {
        const double MinTimeMs = 1.0;
        const double MaxTimeMs = 2000.0;

        public int Index => 0;
        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Delay; // TODO

        public string Name => "Delay";
        public string Info => "Global";
        public string Id => "D33EF934-B34D-4B99-8398-18F1CED6FFA5";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->global.delay;

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Sync, 0 }, { Delay, 1 }, { Step, 1 }, { Feedback, 2 }, { Mix, 3 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int on;
            internal int sync;
            internal int mix;
            internal int step;
            internal int delay;
            internal int feedback;
        }

        static readonly IRelevance RelevanceSync = Relevance.Param((DelayModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.Param((DelayModel m) => m.Sync, (int s) => s == 0);

        public Param On { get; } = new(OnInfo);
        public Param Mix { get; } = new(MixInfo);
        public Param Feedback { get; } = new(FeedbackInfo);
        static readonly ParamInfo MixInfo = ParamInfo.Level(p => &((Native*)p)->mix, 0, nameof(Mix), "Mix", "Dry/wet", true, 0);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", true, false);
        static readonly ParamInfo FeedbackInfo = ParamInfo.Level(p => &((Native*)p)->feedback, 0, nameof(Feedback), "Fbk", "Feedback", true, 0);

        public Param Sync { get; } = new(SyncInfo);
        public Param Step { get; } = new(StepInfo);
        public Param Delay { get; } = new(DelayInfo);
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 1, nameof(Sync), "Sync", "Beat sync", true, false);
        static readonly ParamInfo StepInfo = ParamInfo.Step(p => &((Native*)p)->step, 1, nameof(Step), "Step", "Rate steps", true, 1, 7, RelevanceSync);
        static readonly ParamInfo DelayInfo = ParamInfo.Time(p => &((Native*)p)->delay, 1, nameof(Delay), "Dly", "Delay time", true, 0, MinTimeMs, MaxTimeMs, RelevanceTime);
    }
}