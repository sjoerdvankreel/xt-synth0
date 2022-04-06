using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class GlobalFilterModel : IUIParamGroupModel
    {
        public int Index => 0;
        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.GlobalFilter;

        public string Info => "Global";
        public string Name => $"Filter 3";
        public string Id => "94406474-3773-4C12-B9C7-D45F1ACACF2D";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->global.filter;

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { StateVarPassType, 1 }, { LadderLpHp, 1 }, { CombPlusDelay, 1 }, { Resonance, 2 }, { CombMinDelay, 2 },  { LfoAmount, 3 },
            { CombPlusGain, 5 }, { Frequency, 6 }, { CombMinGain, 6 }, { LfoTarget, 7 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal FilterModel.Native filter;
            internal GlobalModModel.Native mod;
        };

        static readonly IRelevance RelevanceComb = Relevance.Param((GlobalFilterModel m) => m.Type, (FilterType t) => t == FilterType.Comb);
        static readonly IRelevance RelevanceNotComb = Relevance.Param((GlobalFilterModel m) => m.Type, (FilterType t) => t != FilterType.Comb);
        static readonly IRelevance RelevanceLadder = Relevance.Param((GlobalFilterModel m) => m.Type, (FilterType t) => t == FilterType.Ladder);
        static readonly IRelevance RelevanceStateVar = Relevance.Param((GlobalFilterModel m) => m.Type, (FilterType t) => t == FilterType.StateVar);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->filter.on, 0, nameof(On), "On", "Enabled", false, false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<FilterType>(p => &((Native*)p)->filter.type, 0, nameof(Type), "Type", "Filter type", true, FilterModel.TypeNames);

        public Param CombMinGain { get; } = new(CombMinGainInfo);
        public Param CombPlusGain { get; } = new(CombPlusGainInfo);
        public Param CombMinDelay { get; } = new(CombMinDelayInfo);
        public Param CombPlusDelay { get; } = new(CombPlusDelayInfo);
        static readonly ParamInfo CombMinGainInfo = ParamInfo.Mix(p => &((Native*)p)->filter.combMinGain, 0, nameof(CombMinGain), "Gn-", "Comb feedback gain", true, RelevanceComb);
        static readonly ParamInfo CombPlusGainInfo = ParamInfo.Mix(p => &((Native*)p)->filter.combPlusGain, 0, nameof(CombPlusGain), "Gn+", "Comb feedforward gain", true, RelevanceComb);
        static readonly ParamInfo CombMinDelayInfo = ParamInfo.Time(p => &((Native*)p)->filter.combMinDelay, 0, nameof(CombMinDelay), "Dly-", "Comb feedback delay time", true, 0, FilterModel.CombMinDelayMs, FilterModel.CombMaxDelayMs, RelevanceComb);
        static readonly ParamInfo CombPlusDelayInfo = ParamInfo.Time(p => &((Native*)p)->filter.combPlusDelay, 0, nameof(CombPlusDelay), "Dly+", "Comb feedforward delay time", true, 0, FilterModel.CombMinDelayMs, FilterModel.CombMaxDelayMs, RelevanceComb);

        public Param Resonance { get; } = new(ResonanceInfo);
        public Param Frequency { get; } = new(FrequencyInfo);
        public Param LadderLpHp { get; } = new(LadderLpHpInfo);
        public Param StateVarPassType { get; } = new(StateVarPassTypeInfo);
        static readonly ParamInfo ResonanceInfo = ParamInfo.Level(p => &((Native*)p)->filter.resonance, 0, nameof(Resonance), "Res", "Resonance", true, 0, RelevanceNotComb);
        static readonly ParamInfo LadderLpHpInfo = ParamInfo.Level(p => &((Native*)p)->filter.ladderLpHp, 0, nameof(LadderLpHp), "LPHP", "LP/HP crossover", true, 0, RelevanceLadder);
        static readonly ParamInfo StateVarPassTypeInfo = ParamInfo.List<StateVarPassType>(p => &((Native*)p)->filter.stateVarPassType, 0, nameof(StateVarPassType), "Type", "Pass type", true, null, RelevanceStateVar);
        static readonly ParamInfo FrequencyInfo = ParamInfo.Frequency(p => &((Native*)p)->filter.frequency, 0, nameof(Frequency), "Frq", "Cutoff/center frequency", true, 0, FilterModel.MinFreqHz, FilterModel.MaxFreqHz, RelevanceNotComb);

        public Param LfoAmount { get; } = new(LfoAmountInfo);
        public Param LfoTarget { get; } = new(LfoTargetInfo);
        static readonly ParamInfo LfoAmountInfo = ParamInfo.Mix(p => &((Native*)p)->mod.amount, 1, nameof(LfoAmount), "LFO", "LFO 3 amount", true);
        static readonly ParamInfo LfoTargetInfo = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->mod.target, 1, nameof(LfoTarget), "Target", "LFO 3 target", true, FilterModel.TargetNames);
    }
}