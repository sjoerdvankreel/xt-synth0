using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum FilterType
    {
        Biquad,
        Comb
    };

    public enum BiquadType
    {
        LPF,
        HPF,
        BPF,
        BSF
    };

    public enum FilterModTarget
    {
        BiquadFrequency,
        BiquadResonance,
        CombMinGain,
        CombPlusGain,
        CombMinDelay,
        CombPlusDelay
    };

    public unsafe sealed class FilterModel : IUIParamGroupModel
    {
        const int BiquadMinFrequencyHz = 20;
        const int BiquadMaxFrequencyHz = 10000;

        public int Index { get; }
        internal FilterModel(int index) => Index = index;

        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Filter;

        public string Name => $"Filter {Index + 1}";
        public string Id => "33E5297E-8C93-4A0C-810C-CD5E37DB50B2";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->audio.filts[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { BiquadType, 1 }, { CombPlusDelay, 1 }, { BiquadResonance, 2 }, { CombMinDelay, 2 }, { BiquadFrequency, 3 }, { CombPlusGain, 3 },
            { Unit1Amount, 4 }, { Unit2Amount, 5 }, { Unit3Amount, 6 }, { CombMinGain, 7 },
            { Mod1Source, 8 }, { Mod1Target, 9 }, { Mod1Amount, 10 }, { Filter1Amount, 11 },
            { Mod2Source, 12 }, { Mod2Target, 13 }, { Mod2Amount, 14 }, { Filter2Amount, 15 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 96;

            internal int on;
            internal int type;

            internal int combMinGain;
            internal int combPlusGain;
            internal int combMinDelay;
            internal int combPlusDelay;

            internal int biquadType;
            internal int biquadResonance;
            internal int biquadFrequency;
            internal int pad__;

            internal SynthModel.Native.ModModel mod1;
            internal SynthModel.Native.ModModel mod2;
            internal fixed int unitAmount[Model.UnitCount];
            internal fixed int filterAmount[Model.FilterCount];
        };

        static readonly string[] TypeNames = { "Biqd", "Comb" };
        static readonly string[] TargetNames = { "Frq", "Res", "Gn-", "Gn+", "Dly-", "Dly+" };

        static readonly IRelevance Relevance3 = Relevance.Index(i => i > 1);
        static readonly IRelevance Relevance23 = Relevance.Index(i => i > 0);
        static readonly IRelevance RelevanceComb = Relevance.Param((FilterModel m) => m.Type, (FilterType t) => t == FilterType.Comb);
        static readonly IRelevance RelevanceBiquad = Relevance.Param((FilterModel m) => m.Type, (FilterType t) => t == FilterType.Biquad);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<FilterType>(p => &((Native*)p)->type, 0, nameof(Type), "Type", "Filter type", TypeNames);

        public Param CombMinGain { get; } = new(CombMinGainInfo);
        public Param CombPlusGain { get; } = new(CombPlusGainInfo);
        public Param CombMinDelay { get; } = new(CombMinDelayInfo);
        public Param CombPlusDelay { get; } = new(CombPlusDelayInfo);
        static readonly ParamInfo CombMinGainInfo = ParamInfo.Mix(p => &((Native*)p)->combMinGain, 0, nameof(CombMinGain), "Gn-", "Comb feedback gain", RelevanceComb);
        static readonly ParamInfo CombPlusGainInfo = ParamInfo.Mix(p => &((Native*)p)->combPlusGain, 0, nameof(CombPlusGain), "Gn+", "Comb feedforward gain", RelevanceComb);
        static readonly ParamInfo CombMinDelayInfo = ParamInfo.Select(p => &((Native*)p)->combMinDelay, 0, nameof(CombMinDelay), "Dly-", "Comb feedback delay", 1, 255, 128, RelevanceComb);
        static readonly ParamInfo CombPlusDelayInfo = ParamInfo.Select(p => &((Native*)p)->combPlusDelay, 0, nameof(CombPlusDelay), "Dly+", "Comb feedforward delay", 1, 255, 128, RelevanceComb);

        public Param BiquadType { get; } = new(BiquadTypeInfo);
        public Param BiquadResonance { get; } = new(BiquadResonanceInfo);
        public Param BiquadFrequency { get; } = new(BiquadFrequencyInfo);
        static readonly ParamInfo BiquadTypeInfo = ParamInfo.List<BiquadType>(p => &((Native*)p)->biquadType, 0, nameof(BiquadType), "Type", "Biquad type", null, RelevanceBiquad);
        static readonly ParamInfo BiquadResonanceInfo = ParamInfo.Level(p => &((Native*)p)->biquadResonance, 0, nameof(BiquadResonance), "Res", "Resonance", 0, RelevanceBiquad);
        static readonly ParamInfo BiquadFrequencyInfo = ParamInfo.Frequency(p => &((Native*)p)->biquadFrequency, 0, nameof(BiquadFrequency), "Frq", "Cutoff frequency", 0, BiquadMinFrequencyHz, BiquadMaxFrequencyHz, RelevanceBiquad);

        public Param Mod1Amount { get; } = new(Mod1AmountInfo);
        public Param Mod1Target { get; } = new(Mod1TargetInfo);
        public Param Mod1Source { get; } = new(Mod1SourceInfo);
        static readonly ParamInfo Mod1AmountInfo = ParamInfo.Mix(p => &((Native*)p)->mod1.amount, 2, nameof(Mod1Amount), "Amt", "Mod 1 amount");
        static readonly ParamInfo Mod1TargetInfo = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->mod1.target, 2, nameof(Mod1Target), "Target", "Mod 1 target", TargetNames);
        static readonly ParamInfo Mod1SourceInfo = ParamInfo.List<ModSource>(p => &((Native*)p)->mod1.source, 2, nameof(Mod1Source), "Source", "Mod 1 source");

        public Param Mod2Amount { get; } = new(Mod2AmountInfo);
        public Param Mod2Target { get; } = new(Mod2TargetInfo);
        public Param Mod2Source { get; } = new(Mod2SourceInfo);
        static readonly ParamInfo Mod2AmountInfo = ParamInfo.Mix(p => &((Native*)p)->mod2.amount, 2, nameof(Mod2Amount), "Amt", "Mod 2 amount");
        static readonly ParamInfo Mod2TargetInfo = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->mod2.target, 2, nameof(Mod2Target), "Target", "Mod 2 target", TargetNames);
        static readonly ParamInfo Mod2SourceInfo = ParamInfo.List<ModSource>(p => &((Native*)p)->mod2.source, 2, nameof(Mod2Source), "Source", "Mod 2 source");

        public Param Unit1Amount { get; } = new(Unit1AmountInfo);
        public Param Unit2Amount { get; } = new(Unit2AmountInfo);
        public Param Unit3Amount { get; } = new(Unit3AmountInfo);
        static readonly ParamInfo Unit1AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[0], 1, nameof(Unit1Amount), "Ut1", "Unit 1 amount", 0);
        static readonly ParamInfo Unit2AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[1], 1, nameof(Unit2Amount), "Ut2", "Unit 2 amount", 0);
        static readonly ParamInfo Unit3AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[2], 1, nameof(Unit3Amount), "Ut3", "Unit 3 amount", 0);

        public Param Filter1Amount { get; } = new(Filter1AmountInfo);
        public Param Filter2Amount { get; } = new(Filter2AmountInfo);
        static readonly ParamInfo Filter1AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[0], 1, nameof(Filter1Amount), "Ft1", "Filter 1 amount", 0, Relevance23);
        static readonly ParamInfo Filter2AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[1], 1, nameof(Filter2Amount), "Ft2", "Filter 2 amount", 0, Relevance3);
    }
}