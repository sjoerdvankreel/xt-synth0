using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum AmpLfoSource { LFO1, LFO2, LFO3 }
    public enum AmpEnvSource { Env1, Env2, Env3 }

    public unsafe sealed class AmpModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int envSrc;
            internal int lfoSrc;
            internal int panSrc;
            internal int flt1, flt2, flt3;
            internal int unit1, unit2, unit3;
            internal int lvl, pan, lfoAmt, panAmt, pad__;
        }

        public Param Lvl { get; } = new(LvlInfo);
        public Param Pan { get; } = new(PanInfo);
        public Param Flt1 { get; } = new(Flt1Info);
        public Param Flt2 { get; } = new(Flt2Info);
        public Param Flt3 { get; } = new(Flt3Info);
        public Param Unit1 { get; } = new(Unit1Info);
        public Param Unit2 { get; } = new(Unit2Info);
        public Param Unit3 { get; } = new(Unit3Info);
        public Param LfoSrc { get; } = new(LfoSrcInfo);
        public Param LfoAmt { get; } = new(LfoAmtInfo);
        public Param EnvSrc { get; } = new(EnvSrcInfo);
        public Param PanSrc { get; } = new(PanSrcInfo);
        public Param PanAmt { get; } = new(PanAmtInfo);

        public int Index => 0;
        public int Columns => 4;
        public string Name => "Amp";
        public Param Enabled => null;
        public ThemeGroup ThemeGroup => ThemeGroup.Amp;
        public string Id => "F7791FBA-3693-4D71-8EC9-AB507A03FE9A";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->amp;
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { EnvSrc, 0 },
            { Lvl, 4 }, { Pan, 5 }, { Unit1, 6 }, { Flt1, 7 },
            { LfoSrc, 8 }, { PanSrc, 9 }, { Unit2, 10 }, { Flt2, 11 },
            { LfoAmt, 12 }, { PanAmt, 13 }, { Unit3, 14 }, { Flt3, 15 }
        };

        static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, nameof(Pan), nameof(Pan), "Panning");
        static readonly ParamInfo LvlInfo = ParamInfo.Level(p => &((Native*)p)->lvl, nameof(Lvl), nameof(Lvl), "Level", 128);
        static readonly ParamInfo PanAmtInfo = ParamInfo.Mix(p => &((Native*)p)->panAmt, nameof(PanAmt), "Amt", "Pan mod amount");
        static readonly ParamInfo LfoAmtInfo = ParamInfo.Mix(p => &((Native*)p)->lfoAmt, nameof(LfoAmt), "Amt", "Level LFO amount");
        static readonly ParamInfo Flt1Info = ParamInfo.Level(p => &((Native*)p)->flt1, nameof(Flt1), nameof(Flt1), "Filter 1  amount", 0);
        static readonly ParamInfo Flt2Info = ParamInfo.Level(p => &((Native*)p)->flt2, nameof(Flt2), nameof(Flt2), "Filter 2  amount", 0);
        static readonly ParamInfo Flt3Info = ParamInfo.Level(p => &((Native*)p)->flt3, nameof(Flt3), nameof(Flt3), "Filter 3  amount", 0);
        static readonly ParamInfo Unit2Info = ParamInfo.Level(p => &((Native*)p)->unit2, nameof(Unit2), nameof(Unit2), "Unit 2 amount", 0);
        static readonly ParamInfo Unit3Info = ParamInfo.Level(p => &((Native*)p)->unit3, nameof(Unit3), nameof(Unit3), "Unit 3 amount", 0);
        static readonly ParamInfo Unit1Info = ParamInfo.Level(p => &((Native*)p)->unit1, nameof(Unit1), nameof(Unit1), "Unit 1 amount", 255);
        static readonly ParamInfo LfoSrcInfo = ParamInfo.List<AmpLfoSource>(p => &((Native*)p)->lfoSrc, nameof(LfoSrc), "LFO", "Level LFO source");
        static readonly ParamInfo EnvSrcInfo = ParamInfo.List<AmpEnvSource>(p => &((Native*)p)->envSrc, nameof(EnvSrc), "Env", "Level env source");
        static readonly ParamInfo PanSrcInfo = ParamInfo.List<ModSource>(p => &((Native*)p)->panSrc, nameof(PanSrc), nameof(PanSrc), "Pan mod source");
    }
}