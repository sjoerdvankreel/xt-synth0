using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class AmpModel : IUIParamGroupModel
    {
        public int Index => 0;
        public int Columns => 4;
        public Param Enabled => null;
        public ThemeGroup ThemeGroup => ThemeGroup.Amp;

        public string Info => null;
        public string Name => "Amp";
        public string Id => "F7791FBA-3693-4D71-8EC9-AB507A03FE9A";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->voice.amp;

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { Amp, 0 }, { AmpModSource, 1 }, { AmpModAmount, 2 }, { Pan, 3 },
            { Unit1Amount, 4 }, { Unit2Amount, 5 },  { Unit3Amount, 6 }, { PanModSource, 7 },
            { Filter1Amount, 8 }, { Filter2Amount, 9 }, { PanModAmount, 11 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int amp;
            internal int pan;
            internal VoiceModModel.Native ampMod;
            internal VoiceModModel.Native panMod;
            internal fixed int unitAmount[SynthConfig.VoiceUnitCount];
            internal fixed int filterAmount[SynthConfig.VoiceFilterCount];
            internal int pad__;
        }

        public Param Amp { get; } = new(AmpInfo);
        public Param AmpModSource { get; } = new(AmpModSourceInfo);
        public Param AmpModAmount { get; } = new(AmpModAmountInfo);
        static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, 1, nameof(Amp), "Amp", "Amplitude", true, 128);
        static readonly ParamInfo AmpModAmountInfo = ParamInfo.Mix(p => &((Native*)p)->ampMod.amount, 1, nameof(AmpModAmount), "Amt", "Amp mod amount", true);
        static readonly ParamInfo AmpModSourceInfo = ParamInfo.List<VoiceModSource>(p => &((Native*)p)->ampMod.source, 1, nameof(AmpModSource), "Mod", "Amp mod source", true, VoiceModModel.ModSourceNames);

        public Param Pan { get; } = new(PanInfo);
        public Param PanModSource { get; } = new(PanModSourceInfo);
        public Param PanModAmount { get; } = new(PanModAmountInfo);
        static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, 0, nameof(Pan), "Pan", "Panning", true);
        static readonly ParamInfo PanModAmountInfo = ParamInfo.Mix(p => &((Native*)p)->panMod.amount, 0, nameof(PanModAmount), "Amt", "Pan mod amount", true);
        static readonly ParamInfo PanModSourceInfo = ParamInfo.List<VoiceModSource>(p => &((Native*)p)->panMod.source, 0, nameof(PanModSource), "Mod", "Pan mod source", true, VoiceModModel.ModSourceNames);

        public Param Unit1Amount { get; } = new(Unit1AmountInfo);
        public Param Unit2Amount { get; } = new(Unit2AmountInfo);
        public Param Unit3Amount { get; } = new(Unit3AmountInfo);
        static readonly ParamInfo Unit1AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[0], 2, nameof(Unit1Amount), "Ut1", "Unit 1 amount", true, 255);
        static readonly ParamInfo Unit2AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[1], 2, nameof(Unit2Amount), "Ut2", "Unit 2 amount", true, 0);
        static readonly ParamInfo Unit3AmountInfo = ParamInfo.Level(p => &((Native*)p)->unitAmount[2], 2, nameof(Unit3Amount), "Ut3", "Unit 3 amount", true, 0);

        public Param Filter1Amount { get; } = new(Filter1AmountInfo);
        public Param Filter2Amount { get; } = new(Filter2AmountInfo);
        static readonly ParamInfo Filter1AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[0], 2, nameof(Filter1Amount), "Ft1", "Filter 1 amount", true, 0);
        static readonly ParamInfo Filter2AmountInfo = ParamInfo.Level(p => &((Native*)p)->filterAmount[1], 2, nameof(Filter2Amount), "Ft2", "Filter 2 amount", true, 0);
    }
}