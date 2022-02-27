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

            internal SynthModel.Native.ModulationModel modulation1;
            internal SynthModel.Native.ModulationModel modulation2;
            internal fixed int unitAmount[Model.UnitCount];
            internal fixed int filterAmount[Model.FilterCount];
        };

        public Param On { get; } = new(OnInfo);
        public Param Res { get; } = new(ResInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Freq { get; } = new(FreqInfo);
        public Param Amt1 { get; } = new(Amt1Info);
        public Param Amt2 { get; } = new(Amt2Info);
        public Param Src1 { get; } = new(Src1Info);
        public Param Src2 { get; } = new(Src2Info);
        public Param Tgt1 { get; } = new(Tgt1Info);
        public Param Tgt2 { get; } = new(Tgt2Info);
        public Param Flt1 { get; } = new(Flt1Info);
        public Param Flt2 { get; } = new(Flt2Info);
        public Param Unit1 { get; } = new(Unit1Info);
        public Param Unit2 { get; } = new(Unit2Info);
        public Param Unit3 { get; } = new(Unit3Info);
        public Param GMin { get; } = new(GMinInfo);
        public Param GPlus { get; } = new(GPlusInfo);
        public Param BqType { get; } = new(BqTypeInfo);
        public Param DlyMin { get; } = new(DlyMinInfo);
        public Param DlyPlus { get; } = new(DlyPlusInfo);

        public int Columns => 4;
        public int Index { get; }
        public Param Enabled => On;
        public string Name => $"Filter {Index + 1}";
        public ThemeGroup ThemeGroup => ThemeGroup.Filter;
        public string Id => "33E5297E-8C93-4A0C-810C-CD5E37DB50B2";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->audio.filts[Index * Native.Size];

        internal FilterModel(int index) => Index = index;
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { BqType, 1 }, { DlyPlus, 1 }, { Res, 2 }, { DlyMin, 2 }, { Freq, 3 }, { GPlus, 3 },
            { Unit1, 4 }, { Unit2, 5 }, { Unit3, 6 }, { GMin, 7 },
            { Src1, 8 }, { Tgt1, 9 }, { Amt1, 10 }, { Flt1, 11 },
            { Src2, 12 }, { Tgt2, 13 }, { Amt2, 14 }, { Flt2, 15 }
        };

        static readonly string[] TypeNames = { "Biqd", "Comb" };
        static readonly string[] TargetNames = { "Frq", "Res", "Gn-", "Gn+", "Dly-", "Dly+" };

        static readonly IRelevance Relevance3 = Relevance.Index(i => i > 1);
        static readonly IRelevance Relevance23 = Relevance.Index(i => i > 0);
        static readonly IRelevance RelevanceComb = Relevance.Param((FilterModel m) => m.Type, (FilterType t) => t == FilterType.Comb);
        static readonly IRelevance RelevanceBiquad = Relevance.Param((FilterModel m) => m.Type, (FilterType t) => t == FilterType.Biquad);

        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), nameof(On), "Enabled", false);
        static readonly ParamInfo TypeInfo = ParamInfo.List<FilterType>(p => &((Native*)p)->type, 0, nameof(Type), nameof(Type), "Type", TypeNames);
        static readonly ParamInfo Amt1Info = ParamInfo.Mix(p => &((Native*)p)->modulation1.amount, 2, nameof(Amt1), "Amt", "Mod 1 amount");
        static readonly ParamInfo Amt2Info = ParamInfo.Mix(p => &((Native*)p)->modulation2.amount, 2, nameof(Amt2), "Amt", "Mod 2 amount");
        static readonly ParamInfo Src1Info = ParamInfo.List<ModSource>(p => &((Native*)p)->modulation1.source, 2, nameof(Src1), "Source", "Mod 1 source");
        static readonly ParamInfo Src2Info = ParamInfo.List<ModSource>(p => &((Native*)p)->modulation2.source, 2, nameof(Src2), "Source", "Mod 2 source");
        static readonly ParamInfo Tgt1Info = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->modulation1.target, 2, nameof(Tgt1), "Target", "Mod 1 target", TargetNames);
        static readonly ParamInfo Tgt2Info = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->modulation2.target, 2, nameof(Tgt2), "Target", "Mod 2 target", TargetNames);
        static readonly ParamInfo Unit1Info = ParamInfo.Level(p => &((Native*)p)->unitAmount[0], 1, nameof(Unit1), "Ut1", "Unit 1 amount", 0);
        static readonly ParamInfo Unit2Info = ParamInfo.Level(p => &((Native*)p)->unitAmount[1], 1, nameof(Unit2), "Ut2", "Unit 2 amount", 0);
        static readonly ParamInfo Unit3Info = ParamInfo.Level(p => &((Native*)p)->unitAmount[2], 1, nameof(Unit3), "Ut3", "Unit 3 amount", 0);
        static readonly ParamInfo Flt2Info = ParamInfo.Level(p => &((Native*)p)->filterAmount[1], 1, nameof(Flt2), "Ft2", "Filter 2 amount", 0, Relevance3);
        static readonly ParamInfo Flt1Info = ParamInfo.Level(p => &((Native*)p)->filterAmount[0], 1, nameof(Flt1), "Ft1", "Filter 1 amount", 0, Relevance23);
        static readonly ParamInfo ResInfo = ParamInfo.Level(p => &((Native*)p)->biquadResonance, 0, nameof(Res), nameof(Res), "Resonance", 0, RelevanceBiquad);
        static readonly ParamInfo FreqInfo = ParamInfo.Freq(p => &((Native*)p)->biquadFrequency, 0, nameof(Freq), "Frq", "Cutoff frequency", 0, RelevanceBiquad);
        static readonly ParamInfo BqTypeInfo = ParamInfo.List<BiquadType>(p => &((Native*)p)->biquadType, 0, nameof(BqType), nameof(Type), "Biquad type", null, RelevanceBiquad);
        static readonly ParamInfo GMinInfo = ParamInfo.Mix(p => &((Native*)p)->combMinGain, 0, nameof(GMin), "Gn-", "Comb feedback gain", RelevanceComb);
        static readonly ParamInfo GPlusInfo = ParamInfo.Mix(p => &((Native*)p)->combPlusGain, 0, nameof(GPlus), "Gn+", "Comb feedforward gain", RelevanceComb);
        static readonly ParamInfo DlyMinInfo = ParamInfo.Select(p => &((Native*)p)->combMinDelay, 0, nameof(DlyMin), "Dly-", "Comb feedback delay", 1, 255, 128, RelevanceComb);
        static readonly ParamInfo DlyPlusInfo = ParamInfo.Select(p => &((Native*)p)->combPlusDelay, 0, nameof(DlyPlus), "Dly+", "Comb feedforward delay", 1, 255, 128, RelevanceComb);
    }
}