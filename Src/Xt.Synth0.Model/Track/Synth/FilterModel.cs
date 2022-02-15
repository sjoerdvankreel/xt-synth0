using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum FilterModTarget { Freq, Res }
    public enum FilterRoll { Roll6, Roll12, Roll18, Roll24 }
    public enum FilterType { LPF, BPF, HPF, APF, BSF, CPF, CMF }

    public unsafe sealed class FilterModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 56;
            internal int on;
            internal int type;
            internal int roll;
            internal int freq, res;
            internal int amt1, amt2;
            internal int src1, src2;
            internal int tgt1, tgt2;
            internal int unit1, unit2, unit3;
        };

        public Param On { get; } = new(OnInfo);
        public Param Res { get; } = new(ResInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Roll { get; } = new(RollInfo);
        public Param Freq { get; } = new(FreqInfo);
        public Param Amt1 { get; } = new(Amt1Info);
        public Param Amt2 { get; } = new(Amt2Info);
        public Param Src1 { get; } = new(Src1Info);
        public Param Src2 { get; } = new(Src2Info);
        public Param Tgt1 { get; } = new(Tgt1Info);
        public Param Tgt2 { get; } = new(Tgt2Info);
        public Param Unit1 { get; } = new(Unit1Info);
        public Param Unit2 { get; } = new(Unit2Info);
        public Param Unit3 { get; } = new(Unit3Info);

        public int Columns => 3;
        public int Index { get; }
        public Param Enabled => On;
        public string Name => $"Filter {Index + 1}";
        public ThemeGroup ThemeGroup => ThemeGroup.Filter;
        public string Id => "33E5297E-8C93-4A0C-810C-CD5E37DB50B2";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->filters[Index * Native.Size];

        internal FilterModel(int index) => Index = index;
        static readonly string[] Rolls = new[] { "6dB", "12dB", "18dB", "24dB" };

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { Roll, 1},
            { Freq, 3 }, { Res, 4 },
            { Unit1, 6 }, { Unit2, 7 }, { Unit3, 8 },
            { Src1, 9 }, { Tgt1, 10 }, { Amt1, 11 },
            { Src2, 12 }, { Tgt2, 13 }, { Amt2, 14 }
        };

        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), nameof(On), "Enabled", false);
        static readonly ParamInfo Amt1Info = ParamInfo.Mix(p => &((Native*)p)->amt1, nameof(Amt1), "Amt", "Mod 1 amount", 128);
        static readonly ParamInfo Amt2Info = ParamInfo.Mix(p => &((Native*)p)->amt2, nameof(Amt2), "Amt", "Mod 2 amount", 128);
        static readonly ParamInfo ResInfo = ParamInfo.Level(p => &((Native*)p)->res, nameof(Res), nameof(Res), "Resonance", 0);
        static readonly ParamInfo TypeInfo = ParamInfo.List<FilterType>(p => &((Native*)p)->type, nameof(Type), nameof(Type), "Type");
        static readonly ParamInfo Src1Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src1, nameof(Src1), "Source", "Mod 1 source");
        static readonly ParamInfo Src2Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src2, nameof(Src2), "Source", "Mod 2 source");
        static readonly ParamInfo FreqInfo = ParamInfo.Level(p => &((Native*)p)->freq, nameof(Freq), nameof(Freq), "Frequency cutoff", 0);
        static readonly ParamInfo Unit1Info = ParamInfo.Level(p => &((Native*)p)->unit1, nameof(Unit1), nameof(Unit1), "Unit 1 amount", 0);
        static readonly ParamInfo Unit2Info = ParamInfo.Level(p => &((Native*)p)->unit2, nameof(Unit2), nameof(Unit2), "Unit 2 amount", 0);
        static readonly ParamInfo Unit3Info = ParamInfo.Level(p => &((Native*)p)->unit3, nameof(Unit3), nameof(Unit3), "Unit 3 amount", 0);
        static readonly ParamInfo RollInfo = ParamInfo.List<FilterRoll>(p => &((Native*)p)->roll, nameof(Roll), nameof(Roll), "Roll", Rolls);
        static readonly ParamInfo Tgt1Info = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->tgt1, nameof(Tgt1), "Target", "Mod 1 target");
        static readonly ParamInfo Tgt2Info = ParamInfo.List<FilterModTarget>(p => &((Native*)p)->tgt2, nameof(Tgt2), "Target", "Mod 2 target");
    }
}