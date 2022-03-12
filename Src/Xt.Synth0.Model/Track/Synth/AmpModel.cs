using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum AmpLfoSource { LFO1, LFO2, LFO3 }
    public enum AmpEnvSource { Env1, Env2, Env3 }

    public unsafe sealed class AmpModel : IUIParamGroupModel
    {
        public int Index => 0;
        public int Columns => 4;
        public Param Enabled => null;
        public ThemeGroup ThemeGroup => ThemeGroup.Amp;

        public string Name => "Amp";
        public string Id => "F7791FBA-3693-4D71-8EC9-AB507A03FE9A";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->amp;

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { Amp, 0 }, { AmpEnvSource, 1 }, { AmpLfoSource, 2 }, { AmpLfoAmount, 3 },
            { Unit1Amount, 5 },{ Filter1Amount, 6 }, { Panning, 7 },
            { Unit2Amount, 9 }, { Filter2Amount, 10 }, { PanModSource, 11 },
            { Unit3Amount, 13 }, { Filter3Amount, 14 }, { PanModAmount, 15 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int panning;
            internal int panModAmount;
            internal int panModSource;
            internal int pad__;

            internal int amp;
            internal int ampLfoAmount;
            internal int ampLfoSource;
            internal int ampEnvSource;

            internal fixed int unitAmount[SynthConfig.UnitCount];
            internal fixed int filterAmount[SynthConfig.FilterCount];
        }

        public Param Amp { get; } = new(AmpInfo);
        public Param AmpLfoSource { get; } = new(AmpLfoSourceInfo);
        public Param AmpLfoAmount { get; } = new(AmpLfoAmountInfo);
        public Param AmpEnvSource { get; } = new(AmpEnvSourceInfo);
        static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, 1, nameof(Amp), "Amp", "Amplitude", 128);
        static readonly ParamInfo AmpLfoAmountInfo = ParamInfo.Mix(p => &((Native*)p)->ampLfoAmount, 1, nameof(AmpLfoAmount), "Amt", "Level LFO amount");
        static readonly ParamInfo AmpLfoSourceInfo = ParamInfo.List<AmpLfoSource>(p => &((Native*)p)->ampLfoSource, 1, nameof(AmpLfoSource), "LFO", "Amp LFO source");
        static readonly ParamInfo AmpEnvSourceInfo = ParamInfo.List<AmpEnvSource>(p => &((Native*)p)->ampEnvSource, 1, nameof(AmpEnvSource), "Env", "Amp envelope source");

        public Param Panning { get; } = new(PanningInfo);
        public Param PanModSource { get; } = new(PanModSourceInfo);
        public Param PanModAmount { get; } = new(PanModAmountInfo);
        static readonly ParamInfo PanningInfo = ParamInfo.Mix(p => &((Native*)p)->panning, 2, nameof(Panning), "Pan", "Panning");
        static readonly ParamInfo PanModAmountInfo = ParamInfo.Mix(p => &((Native*)p)->panModAmount, 2, nameof(PanModAmount), "Amt", "Pan mod amount");
        static readonly ParamInfo PanModSourceInfo = ParamInfo.List<ModSource>(p => &((Native*)p)->panModSource, 2, nameof(PanModSource), nameof(PanModSource), "Pan mod source", ModModel.ModSourceNames);

        public Param Unit1Amount { get; } = new(Unit1AmountInfo);
        public Param Unit2Amount { get; } = new(Unit2AmountInfo);
        public Param Unit3Amount { get; } = new(Unit3AmountInfo);
        static readonly ParamInfo Unit1AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[0], 0, nameof(Unit1Amount), "Ut1", "Unit 1 amount", 255);
        static readonly ParamInfo Unit2AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[1], 0, nameof(Unit2Amount), "Ut2", "Unit 2 amount", 0);
        static readonly ParamInfo Unit3AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[2], 0, nameof(Unit3Amount), "Ut3", "Unit 3 amount", 0);

        public Param Filter1Amount { get; } = new(Filter1AmountInfo);
        public Param Filter2Amount { get; } = new(Filter2AmountInfo);
        public Param Filter3Amount { get; } = new(Filter3AmountInfo);
        static readonly ParamInfo Filter1AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[0], 0, nameof(Filter1Amount), "Ft1", "Filter 1 amount", 0);
        static readonly ParamInfo Filter2AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[1], 0, nameof(Filter2Amount), "Ft2", "Filter 2 amount", 0);
        static readonly ParamInfo Filter3AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[2], 0, nameof(Filter3Amount), "Ft3", "Filter 3 amount", 0);
    }
}