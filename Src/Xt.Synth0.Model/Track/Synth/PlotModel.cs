using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum PlotType
    {
        Master, Delay, Amp,
        Env1, Env2, Env3,
        LFO1, LFO2, GlobalLFO,
        Unit1, Unit2, Unit3,
        Filter1, Filter2, GlobalFilter
    }

    public unsafe sealed class PlotModel : IUIParamGroupModel
    {
        const int DefaultHold = 100;
        const double MinHoldMs = 1.0;
        const double MaxHoldMs = 3000.0;

        public int Index => 0;
        public int Columns => 3;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Plot;

        public string Info => null;
        public string Name => "Plot";
        public string Id => "BD224A37-6B8E-4EDA-9E49-DE3DD1AF61CE";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->global.plot;

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>() 
        { 
            { On, -1 }, 
            { Type, 0 }, 
            { Spectrum, 1 }, 
            { Hold, 2 } 
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal int on;
            internal int hold;
            internal int type;
            internal int spectrum;
        }

        static readonly string[] PlotTypeNames = { "Master", "Delay", "Amp", "Env1", "Env2", "Env3", "LFO1", "LFO2", "LFO3", "Unit1", "Unit2", "Unit3", "Filter1", "Filter2", "Filter3" };

        static readonly IRelevance RelevanceHold = Relevance.Param((PlotModel m) => m.Type, (PlotType t) => t < PlotType.LFO1);
        static readonly IRelevance RelevanceSpectrum = Relevance.Param((PlotModel m) => m.Type, (PlotType t) => t >= PlotType.LFO1 || t == PlotType.Master || t == PlotType.Delay);

        public Param On { get; } = new(OnInfo);
        public Param Type { get; } = new(TypeInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), "On", "Enabled", true, true);
        static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, 1, nameof(Type), "Type", "Type", true, PlotTypeNames);

        public Param Hold { get; } = new(HoldInfo);
        public Param Spectrum { get; } = new(SpectrumInfo);
        static readonly ParamInfo SpectrumInfo = ParamInfo.Toggle(p => &((Native*)p)->spectrum, 1, nameof(Spectrum), "Spec", "Spectrum", true, false, RelevanceSpectrum);
        static readonly ParamInfo HoldInfo = ParamInfo.Time(p => &((Native*)p)->hold, 1, nameof(Hold), "Hold", "Hold key time", true, DefaultHold, MinHoldMs, MaxHoldMs, RelevanceHold);
    }
}