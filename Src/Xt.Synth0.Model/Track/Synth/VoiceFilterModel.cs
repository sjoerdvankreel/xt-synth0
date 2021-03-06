using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class VoiceFilterModel : IUIParamGroupModel
    {
        public int Index { get; }
        internal VoiceFilterModel(int index) => Index = index;

        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.VoiceFilter;

        public string Info => null;
        public string Name => $"Filter {Index + 1}";
        public string Id => "33E5297E-8C93-4A0C-810C-CD5E37DB50B2";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->voice.audio.filters[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { StateVarPassType, 1 }, { LadderDrive, 1 }, { CombPlusDelay, 1 }, { LadderLpHp, 2 }, { CombMinDelay, 2 }, { Resonance, 3 }, { CombPlusGain, 3 },
            { Mod1Source, 4 }, { Mod1Target, 5 }, { Mod1Amount, 6 }, { Frequency, 7 }, { CombMinGain, 7 },
            { Mod2Source, 8 }, { Mod2Target, 9 }, { Mod2Amount, 10 }, { KeyboardTrack, 11 },
            { Unit1Amount, 12 }, { Unit2Amount, 13 }, { Unit3Amount, 14 }, { Filter1Amount, 15 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 104;

            internal FilterModel.Native filter;
            internal TargetModsModel.Native mods;
            internal int keyboardTrack;
            internal fixed int unitAmount[SynthConfig.VoiceUnitCount];
            internal fixed int filterAmount[SynthConfig.VoiceFilterCount];
        };

        static readonly IRelevance Relevance2 = Relevance.Index(i => i > 0);
        static readonly IRelevance RelevanceComb = Relevance.Param((VoiceFilterModel m) => m.Type, (FilterType t) => t == FilterType.Comb);
        static readonly IRelevance RelevanceNotComb = Relevance.Param((VoiceFilterModel m) => m.Type, (FilterType t) => t != FilterType.Comb);
        static readonly IRelevance RelevanceLadder = Relevance.Param((VoiceFilterModel m) => m.Type, (FilterType t) => t == FilterType.Ladder);
        static readonly IRelevance RelevanceStateVar = Relevance.Param((VoiceFilterModel m) => m.Type, (FilterType t) => t == FilterType.StateVar);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->filter.on, 1, nameof(On), "On", "Enabled", false, false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<FilterType>(p => &((Native*)p)->filter.type, 1, nameof(Type), "Type", "Filter type", true, FilterModel.TypeNames);

        public Param CombMinGain { get; } = new(CombMinGainInfo);
        public Param CombPlusGain { get; } = new(CombPlusGainInfo);
        public Param CombMinDelay { get; } = new(CombMinDelayInfo);
        public Param CombPlusDelay { get; } = new(CombPlusDelayInfo);
        static readonly ParamInfo CombMinGainInfo = ParamInfo.Mix(p => &((Native*)p)->filter.combMinGain, 1, nameof(CombMinGain), "Gn-", "Comb feedback gain", true, RelevanceComb);
        static readonly ParamInfo CombPlusGainInfo = ParamInfo.Mix(p => &((Native*)p)->filter.combPlusGain, 1, nameof(CombPlusGain), "Gn+", "Comb feedforward gain", true, RelevanceComb);
        static readonly ParamInfo CombMinDelayInfo = ParamInfo.Time(p => &((Native*)p)->filter.combMinDelay, 1, nameof(CombMinDelay), "Dly-", "Comb feedback delay time", true, 0, FilterModel.CombMinDelayMs, FilterModel.CombMaxDelayMs, RelevanceComb);
        static readonly ParamInfo CombPlusDelayInfo = ParamInfo.Time(p => &((Native*)p)->filter.combPlusDelay, 1, nameof(CombPlusDelay), "Dly+", "Comb feedforward delay time", true, 0, FilterModel.CombMinDelayMs, FilterModel.CombMaxDelayMs, RelevanceComb);

        public Param Resonance { get; } = new(ResonanceInfo);
        public Param Frequency { get; } = new(FrequencyInfo);
        public Param LadderLpHp { get; } = new(LadderLpHpInfo);
        public Param LadderDrive { get; } = new(LadderDriveInfo);
        public Param KeyboardTrack { get; } = new(KeyboardTrackInfo);
        public Param StateVarPassType { get; } = new(StateVarPassTypeInfo);
        static readonly ParamInfo ResonanceInfo = ParamInfo.Level(p => &((Native*)p)->filter.resonance, 1, nameof(Resonance), "Res", "Resonance", true, 0, RelevanceNotComb);
        static readonly ParamInfo LadderDriveInfo = ParamInfo.Level(p => &((Native*)p)->filter.ladderDrive, 1, nameof(LadderDrive), "Drv", "Drive", true, 0, RelevanceLadder);
        static readonly ParamInfo KeyboardTrackInfo = ParamInfo.Mix(p => &((Native*)p)->keyboardTrack, 1, nameof(KeyboardTrack), "Kbd", "Keyboard tracking amount", true, RelevanceNotComb);
        static readonly ParamInfo LadderLpHpInfo = ParamInfo.Level(p => &((Native*)p)->filter.ladderLpHp, 1, nameof(LadderLpHp), "LPHP", "LP/HP crossover", true, 0, RelevanceLadder);
        static readonly ParamInfo StateVarPassTypeInfo = ParamInfo.List<StateVarPassType>(p => &((Native*)p)->filter.stateVarPassType, 1, nameof(StateVarPassType), "Type", "Pass type", true, null, RelevanceStateVar);
        static readonly ParamInfo FrequencyInfo = ParamInfo.Frequency(p => &((Native*)p)->filter.frequency, 1, nameof(Frequency), "Frq", "Cutoff/center frequency", true, 0, FilterModel.MinFreqHz, FilterModel.MaxFreqHz, RelevanceNotComb);

        public Param Mod1Amount { get; } = new(Mod1AmountInfo);
        public Param Mod1Target { get; } = new(Mod1TargetInfo);
        public Param Mod1Source { get; } = new(Mod1SourceInfo);
        static readonly ParamInfo Mod1AmountInfo = ParamInfo.Mix(p => &((Native*)p)->mods.mod1.mod.amount, 2, nameof(Mod1Amount), "Amt", "Mod 1 amount", true);
        static readonly ParamInfo Mod1TargetInfo = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->mods.mod1.target, 2, nameof(Mod1Target), "Target", "Mod 1 target", true, FilterModel.TargetNames);
        static readonly ParamInfo Mod1SourceInfo = ParamInfo.List<VoiceModSource>(p => &((Native*)p)->mods.mod1.mod.source, 2, nameof(Mod1Source), "Source", "Mod 1 source", true, VoiceModModel.ModSourceNames);

        public Param Mod2Amount { get; } = new(Mod2AmountInfo);
        public Param Mod2Target { get; } = new(Mod2TargetInfo);
        public Param Mod2Source { get; } = new(Mod2SourceInfo);
        static readonly ParamInfo Mod2AmountInfo = ParamInfo.Mix(p => &((Native*)p)->mods.mod2.mod.amount, 2, nameof(Mod2Amount), "Amt", "Mod 2 amount", true);
        static readonly ParamInfo Mod2TargetInfo = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->mods.mod2.target, 2, nameof(Mod2Target), "Target", "Mod 2 target", true, FilterModel.TargetNames);
        static readonly ParamInfo Mod2SourceInfo = ParamInfo.List<VoiceModSource>(p => &((Native*)p)->mods.mod2.mod.source, 2, nameof(Mod2Source), "Source", "Mod 2 source", true, VoiceModModel.ModSourceNames);

        public Param Unit1Amount { get; } = new(Unit1AmountInfo);
        public Param Unit2Amount { get; } = new(Unit2AmountInfo);
        public Param Unit3Amount { get; } = new(Unit3AmountInfo);
        public Param Filter1Amount { get; } = new(Filter1AmountInfo);
        static readonly ParamInfo Unit1AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[0], 0, nameof(Unit1Amount), "Ut1", "Unit 1 amount", true, 0);
        static readonly ParamInfo Unit2AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[1], 0, nameof(Unit2Amount), "Ut2", "Unit 2 amount", true, 0);
        static readonly ParamInfo Unit3AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[2], 0, nameof(Unit3Amount), "Ut3", "Unit 3 amount", true, 0);
        static readonly ParamInfo Filter1AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[0], 0, nameof(Filter1Amount), "Ft1", "Filter 1 amount", true, 0, Relevance2);
    }
}