using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum EnvType { DAHDSR, DAHDR }
    public enum SlopeType { Lin, Log, Inv, Sin, Cos }

    public unsafe sealed class EnvModel : IUIParamGroupModel
    {
        const double MinTimeMs = 0.0;
        const double MaxTimeMs = 10000.0;

        public int Index { get; }
        internal EnvModel(int index) => Index = index;

        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Env;

        public string Name => $"Env {Index + 1}";
        public string Info => Index == 0 ? "Amp" : null;
        public string Id => "A7FF2DD9-62D5-4426-8530-02C60710237D";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->voice.cv.envs[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { DelayTime, 1 }, { DelayStep, 1 }, { AttackSlope, 2 }, { AttackTime, 3 }, { AttackStep, 3 },
            { Sync, 4 }, { HoldTime, 5 }, { HoldStep, 5 }, { DecaySlope, 6 }, { DecayTime, 7 }, { DecayStep, 7 },
            { Invert, 8 }, { Sustain, 9 }, { ReleaseSlope, 10 }, { ReleaseTime, 11 }, { ReleaseStep, 11 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 72;

            internal int type;
            internal int on;
            internal int sync;
            internal int invert;

            internal int delayTime;
            internal int attackTime;
            internal int holdTime;
            internal int decayTime;
            internal int releaseTime;

            internal int delayStep;
            internal int attackStep;
            internal int holdStep;
            internal int decayStep;
            internal int releaseStep;

            internal int sustain;
            internal int decaySlope;
            internal int attackSlope;
            internal int releaseSlope;
        }

        static readonly IRelevance RelevanceSync = Relevance.Param((EnvModel m) => m.Sync, (int s) => s == 1);
        static readonly IRelevance RelevanceTime = Relevance.Param((EnvModel m) => m.Sync, (int s) => s == 0);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Sync { get; } = new(SyncInfo);
        public Param Invert { get; } = new(InvertInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", false, false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<EnvType>(p => &((Native*)p)->type, 0, nameof(Type), "Type", "Type", false);
        static readonly ParamInfo SyncInfo = ParamInfo.Toggle(p => &((Native*)p)->sync, 0, nameof(Sync), "Sync", "Beat sync", false, false);
        static readonly ParamInfo InvertInfo = ParamInfo.Toggle(p => &((Native*)p)->invert, 0, nameof(Invert), "Invert", "Invert", false, false);

        public Param HoldTime { get; } = new(HoldTimeInfo);
        public Param DelayTime { get; } = new(DelayTimeInfo);
        public Param DecayTime { get; } = new(DecayTimeInfo);
        public Param AttackTime { get; } = new(AttackTimeInfo);
        public Param ReleaseTime { get; } = new(ReleaseTimeInfo);
        static readonly ParamInfo HoldTimeInfo = ParamInfo.Time(p => &((Native*)p)->holdTime, 2, nameof(HoldTime), "Hld", "Hold time", false, 0, MinTimeMs, MaxTimeMs, RelevanceTime);
        static readonly ParamInfo DelayTimeInfo = ParamInfo.Time(p => &((Native*)p)->delayTime, 2, nameof(DelayTime), "Dly", "Delay time", false, 0, MinTimeMs, MaxTimeMs, RelevanceTime);
        static readonly ParamInfo DecayTimeInfo = ParamInfo.Time(p => &((Native*)p)->decayTime, 1, nameof(DecayTime), "D", "Decay time", false, 18, MinTimeMs, MaxTimeMs, RelevanceTime);
        static readonly ParamInfo AttackTimeInfo = ParamInfo.Time(p => &((Native*)p)->attackTime, 1, nameof(AttackTime), "A", "Attack time", false, 7, MinTimeMs, MaxTimeMs, RelevanceTime);
        static readonly ParamInfo ReleaseTimeInfo = ParamInfo.Time(p => &((Native*)p)->releaseTime, 1, nameof(ReleaseTime), "R", "Release time", false, 36, MinTimeMs, MaxTimeMs, RelevanceTime);

        public Param HoldStep { get; } = new(HoldStepInfo);
        public Param DelayStep { get; } = new(DelayStepInfo);
        public Param DecayStep { get; } = new(DecayStepInfo);
        public Param AttackStep { get; } = new(AttackStepInfo);
        public Param ReleaseStep { get; } = new(ReleaseStepInfo);
        static readonly ParamInfo HoldStepInfo = ParamInfo.Step(p => &((Native*)p)->holdStep, 2, nameof(HoldStep), "Hld", "Hold steps", false, 0, 0, RelevanceSync);
        static readonly ParamInfo DecayStepInfo = ParamInfo.Step(p => &((Native*)p)->decayStep, 1, nameof(DecayStep), "D", "Decay steps", false, 0, 11, RelevanceSync);
        static readonly ParamInfo DelayStepInfo = ParamInfo.Step(p => &((Native*)p)->delayStep, 2, nameof(DelayStep), "Dly", "Delay steps", false, 0, 0, RelevanceSync);
        static readonly ParamInfo AttackStepInfo = ParamInfo.Step(p => &((Native*)p)->attackStep, 1, nameof(AttackStep), "A", "Attack steps", false, 0, 1, RelevanceSync);
        static readonly ParamInfo ReleaseStepInfo = ParamInfo.Step(p => &((Native*)p)->releaseStep, 1, nameof(ReleaseStep), "R", "Release steps", false, 0, 15, RelevanceSync);

        public Param Sustain { get; } = new(SustainInfo);
        public Param AttackSlope { get; } = new(AttackSlopeInfo);
        public Param DecaySlope { get; } = new(DecaySlopeInfo);
        public Param ReleaseSlope { get; } = new(ReleaseSlopeInfo);
        static readonly ParamInfo SustainInfo = ParamInfo.Level(p => &((Native*)p)->sustain, 2, nameof(Sustain), "S", "Sustain level", false, 128);
        static readonly ParamInfo DecaySlopeInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->decaySlope, 1, "DecaySlope", "Slp", "Decay slope", false);
        static readonly ParamInfo AttackSlopeInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->attackSlope, 1, "AttackSlope", "Slp", "Attack slope", false);
        static readonly ParamInfo ReleaseSlopeInfo = ParamInfo.List<SlopeType>(p => &((Native*)p)->releaseSlope, 1, "ReleaseSlope", "Slp", "Release slope", false);
    }
}