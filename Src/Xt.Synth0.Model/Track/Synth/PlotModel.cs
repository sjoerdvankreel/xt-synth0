using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum PlotType { Synth, Amp, Env1, Env2, Env3, LFO1, LFO2, LFO3, Unit1, Unit2, Unit3 }

    public unsafe sealed class PlotModel : IUIParamGroupModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native { internal int on, spec, type, hold; }

        public Param On { get; } = new(OnInfo);
        public Param Spec { get; } = new(SpecInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Hold { get; } = new(HoldInfo);

        public int Index => 0;
        public int Columns => 3;
        public Param Enabled => On;
        public string Name => "Plot";
        public ThemeGroup ThemeGroup => ThemeGroup.Plot;
        public string Id => "BD224A37-6B8E-4EDA-9E49-DE3DD1AF61CE";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->plot;
        public IDictionary<Param, int> Layout => new Dictionary<Param, int>() { { On, -1 }, { Type, 0 }, { Spec, 1 }, { Hold, 2 } };

        static bool HoldRelevant(PlotType t) => t < PlotType.LFO1;
        static bool SpecRelevant(PlotType t) => t >= PlotType.LFO1 || t == PlotType.Synth;
        static readonly IRelevance RelevanceSpec = Relevance.Param((PlotModel m) => m.Type, (PlotType t) => SpecRelevant(t));
        static readonly IRelevance RelevanceHold = Relevance.Any(
            Relevance.Param((PlotModel m) => m.Type, (PlotType t) => !SpecRelevant(t)),
            Relevance.All(
                Relevance.Param((PlotModel m) => m.Type, (PlotType t) => HoldRelevant(t)),
                Relevance.Param((PlotModel m) => m.Spec, (int s) => s == 0)));

        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 0, nameof(On), nameof(On), "Enabled", true);
        static readonly ParamInfo TypeInfo = ParamInfo.List<PlotType>(p => &((Native*)p)->type, 1, nameof(Type), nameof(Type), "Source");
        static readonly ParamInfo SpecInfo = ParamInfo.Toggle(p => &((Native*)p)->spec, 1, nameof(Spec), nameof(Spec), "Spectrum", false, RelevanceSpec);
        static readonly ParamInfo HoldInfo = ParamInfo.Time(p => &((Native*)p)->hold, 1, nameof(Hold), nameof(Hold), "Hold key time", 1, 180, 26, RelevanceHold);
    }
}