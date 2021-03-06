using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public unsafe sealed class SynthModel : AutomatableModel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        public ref struct Native
        {
            internal VoiceModel.Native voice;
            internal GlobalModel.Native global;
        }

        public AmpModel Amp { get; } = new();
        public PlotModel Plot { get; } = new();
        public DelayModel Delay { get; } = new();
        public MasterModel Master { get; } = new();
        public GlobalFilterModel GlobalFilter { get; } = new();
        public LfoModel GlobalLfo { get; } = new(true, SynthConfig.VoiceLfoCount);
        public IReadOnlyList<LfoModel> Lfos = new ReadOnlyCollection<LfoModel>(MakeLfos());
        public IReadOnlyList<EnvModel> Envs = new ReadOnlyCollection<EnvModel>(MakeEnvs());
        public IReadOnlyList<UnitModel> Units = new ReadOnlyCollection<UnitModel>(MakeUnits());
        public IReadOnlyList<VoiceFilterModel> Filters = new ReadOnlyCollection<VoiceFilterModel>(MakeFilters());

        static IList<EnvModel> MakeEnvs() => Enumerable.Range(0, SynthConfig.VoiceEnvCount).Select(i => new EnvModel(i)).ToList();
        static IList<UnitModel> MakeUnits() => Enumerable.Range(0, SynthConfig.VoiceUnitCount).Select(i => new UnitModel(i)).ToList();
        static IList<LfoModel> MakeLfos() => Enumerable.Range(0, SynthConfig.VoiceLfoCount).Select(i => new LfoModel(false, i)).ToList();
        static IList<VoiceFilterModel> MakeFilters() => Enumerable.Range(0, SynthConfig.VoiceFilterCount).Select(i => new VoiceFilterModel(i)).ToList();

        public override string Id => "8D6AB9FB-19DB-4F77-B56C-9E72AB67341F";
        public override IReadOnlyList<IParamGroupModel> Groups => Units
            .Concat<IParamGroupModel>(Envs)
            .Concat(Lfos).Concat(Filters)
            .Concat(new IParamGroupModel[] { Plot, Amp, GlobalLfo, GlobalFilter, Delay, Master }).ToArray();

        public SynthModel()
        {
            Envs[0].On.Value = 1;
            Units[0].On.Value = 1;
            if (AutoParams.Count != SynthConfig.SynthParamCount)
                throw new InvalidOperationException();
        }

        public void Bind(Native* native, int** binding, Native* nativeVoices, int** voiceBindings)
        {
            Bind(native, binding);
            for (int i = 0; i < SynthConfig.SynthMaxVoices; i++)
                Bind(&nativeVoices[i], &voiceBindings[i * SynthConfig.SynthParamCount]);
        }
    }
}