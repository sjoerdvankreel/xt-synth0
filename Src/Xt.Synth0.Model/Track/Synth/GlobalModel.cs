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
            internal int envSrc, lfoSrc;
            internal int amp, keyAmt, lfoAmt, pad__;
        }

        public Param Amp { get; } = new(AmpInfo);
        public Param KeyAmt { get; } = new(KeyAmtInfo);
        public Param LfoSrc { get; } = new(LfoSrcInfo);
        public Param LfoAmt { get; } = new(LfoAmtInfo);
        public Param EnvSrc { get; } = new(EnvSrcInfo);

        public int Index => 0;
        public int Columns => 3;
        public Param Enabled => null;
        public string Name => "Global";
        public ThemeGroup ThemeGroup => ThemeGroup.Global;
        public string Id => "F7791FBA-3693-4D71-8EC9-AB507A03FE9A";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->global;
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { Amp, 0 }, { LfoSrc, 1 }, { LfoAmt, 2 },
            { EnvSrc, 4 }, { KeyAmt, 5 }
        };

        static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), nameof(Amp), "Amplitude", 128);
        static readonly ParamInfo LfoAmtInfo = ParamInfo.Mix(p => &((Native*)p)->lfoAmt, nameof(LfoAmt), "Amt", "Amp LFO amount", 128);
        static readonly ParamInfo KeyAmtInfo = ParamInfo.Mix(p => &((Native*)p)->keyAmt, nameof(KeyAmt), "Key", "Key velocity amount", 128);
        static readonly ParamInfo LfoSrcInfo = ParamInfo.List<GlobalAmpLfo>(p => &((Native*)p)->lfoSrc, nameof(LfoSrc), "LFO", "Amp LFO source");
        static readonly ParamInfo EnvSrcInfo = ParamInfo.List<GlobalAmpEnv>(p => &((Native*)p)->envSrc, nameof(EnvSrc), "Env", "Amp env source");
    }
}