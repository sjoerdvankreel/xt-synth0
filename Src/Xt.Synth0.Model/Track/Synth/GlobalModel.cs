using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum GlobalAmpLfo { LFO1, LFO2 }
    public enum GlobalAmpEnv { Env1, Env2, Env3 }

    public unsafe sealed class GlobalModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int envSrc;
            internal int lfoSrc;
            internal int amp, lfoAmt;
            internal int flt1, flt2, flt3;
            internal int unit1, unit2, unit3;
        }

        public Param Amp { get; } = new(AmpInfo);
        public Param Flt1 { get; } = new(Flt1Info);
        public Param Flt2 { get; } = new(Flt2Info);
        public Param Flt3 { get; } = new(Flt3Info);
        public Param Unit1 { get; } = new(Unit1Info);
        public Param Unit2 { get; } = new(Unit2Info);
        public Param Unit3 { get; } = new(Unit3Info);
        public Param LfoSrc { get; } = new(LfoSrcInfo);
        public Param LfoAmt { get; } = new(LfoAmtInfo);
        public Param EnvSrc { get; } = new(EnvSrcInfo);

        public int Index => 0;
        public int Columns => 4;
        public Param Enabled => null;
        public string Name => "Global";
        public ThemeGroup ThemeGroup => ThemeGroup.Global;
        public string Id => "F7791FBA-3693-4D71-8EC9-AB507A03FE9A";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { EnvSrc, 0 }, { Amp, 1 }, { LfoSrc, 2 }, { LfoAmt, 3 },
            { Unit1, 4 }, { Unit2, 5 }, { Unit3, 6 },
            { Flt1, 8 }, { Flt2, 9 }, { Flt3, 10 }
        };

        static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), nameof(Amp), "Amplitude", 128);
        static readonly ParamInfo LfoAmtInfo = ParamInfo.Mix(p => &((Native*)p)->lfoAmt, nameof(LfoAmt), "Amt", "Amp LFO amount", 128);
        static readonly ParamInfo Flt1Info = ParamInfo.Level(p => &((Native*)p)->flt1, nameof(Flt1), nameof(Flt1), "Filter 1  amount", 0);
        static readonly ParamInfo Flt2Info = ParamInfo.Level(p => &((Native*)p)->flt2, nameof(Flt2), nameof(Flt2), "Filter 2  amount", 0);
        static readonly ParamInfo Flt3Info = ParamInfo.Level(p => &((Native*)p)->flt3, nameof(Flt3), nameof(Flt3), "Filter 3  amount", 0);
        static readonly ParamInfo Unit2Info = ParamInfo.Level(p => &((Native*)p)->unit2, nameof(Unit2), nameof(Unit2), "Unit 2 amount", 0);
        static readonly ParamInfo Unit3Info = ParamInfo.Level(p => &((Native*)p)->unit3, nameof(Unit3), nameof(Unit3), "Unit 3 amount", 0);
        static readonly ParamInfo Unit1Info = ParamInfo.Level(p => &((Native*)p)->unit1, nameof(Unit1), nameof(Unit1), "Unit 1 amount", 255);
        static readonly ParamInfo LfoSrcInfo = ParamInfo.List<GlobalAmpLfo>(p => &((Native*)p)->lfoSrc, nameof(LfoSrc), "LFO", "Amp LFO source");
        static readonly ParamInfo EnvSrcInfo = ParamInfo.List<GlobalAmpEnv>(p => &((Native*)p)->envSrc, nameof(EnvSrc), "Env", "Amp env source");
    }
}