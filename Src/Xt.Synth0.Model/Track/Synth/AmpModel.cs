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
            internal int lvlSrc;
            internal int panSrc;
            internal fixed int units[Model.UnitCount];
            internal fixed int flts[Model.FilterCount];
            internal int lvl, pan, lvlAmt, panAmt, pad__;
        }

        public Param Lvl { get; } = new(LvlInfo);
        public Param Pan { get; } = new(PanInfo);
        public Param Flt1 { get; } = new(Flt1Info);
        public Param Flt2 { get; } = new(Flt2Info);
        public Param Flt3 { get; } = new(Flt3Info);
        public Param Unit1 { get; } = new(Unit1Info);
        public Param Unit2 { get; } = new(Unit2Info);
        public Param Unit3 { get; } = new(Unit3Info);
        public Param LvlSrc { get; } = new(LvlSrcInfo);
        public Param LvlAmt { get; } = new(LvlAmtInfo);
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
            { Lvl, 0 }, { EnvSrc, 1 }, { LvlSrc, 2 }, { LvlAmt, 3 },
            { Unit1, 5 },{ Flt1, 6 }, { Pan, 7 },
            { Unit2, 9 }, { Flt2, 10 }, { PanSrc, 11 },
            { Unit3, 13 }, { Flt3, 14 }, { PanAmt, 15 }
        };

        static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, 2, nameof(Pan), nameof(Pan), "Panning");
        static readonly ParamInfo LvlInfo = ParamInfo.Level(p => &((Native*)p)->lvl, 1, nameof(Lvl), nameof(Lvl), "Level", 128);
        static readonly ParamInfo PanAmtInfo = ParamInfo.Mix(p => &((Native*)p)->panAmt, 2, nameof(PanAmt), "Amt", "Pan mod amount");
        static readonly ParamInfo LvlAmtInfo = ParamInfo.Mix(p => &((Native*)p)->lvlAmt, 1, nameof(LvlAmt), "Amt", "Level LFO amount");
        static readonly ParamInfo Flt1Info = ParamInfo.Level(p => &((Native*)p)->flts[0], 0, nameof(Flt1), "Ft1", "Filter 1 amount", 0);
        static readonly ParamInfo Flt2Info = ParamInfo.Level(p => &((Native*)p)->flts[1], 0, nameof(Flt2), "Ft2", "Filter 2 amount", 0);
        static readonly ParamInfo Flt3Info = ParamInfo.Level(p => &((Native*)p)->flts[2], 0, nameof(Flt3), "Ft3", "Filter 3 amount", 0);
        static readonly ParamInfo Unit1Info = ParamInfo.Level(p => &((Native*)p)->units[0], 0, nameof(Unit1), "Ut1", "Unit 1 amount", 255);
        static readonly ParamInfo Unit2Info = ParamInfo.Level(p => &((Native*)p)->units[1], 0, nameof(Unit2), "Ut2", "Unit 2 amount", 0);
        static readonly ParamInfo Unit3Info = ParamInfo.Level(p => &((Native*)p)->units[2], 0, nameof(Unit3), "Ut3", "Unit 3 amount", 0);
        static readonly ParamInfo LvlSrcInfo = ParamInfo.List<AmpLfoSource>(p => &((Native*)p)->lvlSrc, 1, nameof(LvlSrc), "LFO", "Level LFO source");
        static readonly ParamInfo EnvSrcInfo = ParamInfo.List<AmpEnvSource>(p => &((Native*)p)->envSrc, 1, nameof(EnvSrc), "Env", "Level env source");
        static readonly ParamInfo PanSrcInfo = ParamInfo.List<ModSource>(p => &((Native*)p)->panSrc, 2, nameof(PanSrc), nameof(PanSrc), "Pan mod source");
    }
}