using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class MasterModel : IUIParamGroupModel
    {
        public int Index => 0;
        public int Columns => 4;
        public Param Enabled => null;
        public ThemeGroup ThemeGroup => ThemeGroup.Amp;

        public string Info => "Global";
        public string Name => "Master";
        public string Id => "B040B7DD-1C8D-4589-86F0-576EE0DBF268";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->global.master;

        public IDictionary<Param, int> Layout => new Dictionary<Param, int> { { Amp, 0 }, { AmpLfo, 1 }, { Pan, 2 }, { PanLfo, 3 } };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int amp;
            internal int ampLfo;
            internal int pan;
            internal int panLfo;
        }

        public Param Amp { get; } = new(AmpInfo);
        public Param AmpLfo { get; } = new(AmpLfoInfo);
        static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, 1, nameof(Amp), "Amp", "Amplitude", true, 128);
        static readonly ParamInfo AmpLfoInfo = ParamInfo.Mix(p => &((Native*)p)->ampLfo, 1, nameof(AmpLfo), "LFO", "Amp LFO 3 amount", true);

        public Param Pan { get; } = new(PanInfo);
        public Param PanLfo { get; } = new(PanLfoInfo);
        static readonly ParamInfo PanInfo = ParamInfo.Level(p => &((Native*)p)->pan, 0, nameof(Pan), "Pan", "Panning", true, 128);
        static readonly ParamInfo PanLfoInfo = ParamInfo.Mix(p => &((Native*)p)->panLfo, 0, nameof(PanLfo), "LFO", "Pan LFO 3 amount", true);
    }
}