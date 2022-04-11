using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum BlepType { Saw, Pulse, Triangle }
    public enum FMType { Sine, Saw, Square, Triangle }
    public enum UnitType { Sine, Additive, PolyBlep, FM }

    public enum UnitModTarget
    {
        Amp,
        Pan,
        Phase,
        Pitch,
        Frequency,
        FMIndex,
        FMDamping,
        BlepPulseWidth,
        AdditiveRolloff
    }

    public unsafe sealed class UnitModel : IUIParamGroupModel
    {
        public int Index { get; }
        internal UnitModel(int index) => Index = index;

        public int Columns => 4;
        public Param Enabled => On;
        public ThemeGroup ThemeGroup => ThemeGroup.Unit;

        public string Info => null;
        public string Name => $"Unit {Index + 1}";
        public string Id => "3DACD0A4-9688-4FA9-9CA3-B8A0E49A45E5";
        public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
        public void* Address(void* parent) => &((SynthModel.Native*)parent)->voice.audio.units[Index * Native.Size];

        public IDictionary<Param, int> Layout => new Dictionary<Param, int>
        {
            { On, -1 },
            { Type, 0 }, { AdditiveSub, 1 }, { FMDamping, 1 }, { BlepType, 1 }, { Amp, 2 }, { Pan, 3 },
            { AdditivePartials, 4 }, { FMCarrier, 4 }, { AdditiveStep, 5 }, { FMModulator, 5 }, { AdditiveRolloff, 6 }, { FMIndex, 6 }, { BlepPulseWidth, 6 }, { Octave, 7 },
            { Mod1Source, 8 }, { Mod1Target, 9 }, { Mod1Amount, 10 }, { Note, 11 },
            { Mod2Source, 12 },  { Mod2Target, 13 }, { Mod2Amount, 14}, { Detune, 15 }
        };

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 104;

            internal int on;
            internal int type;
            internal int amp;
            internal int pan;

            internal int note;
            internal int octave;
            internal int detune;
            internal int pad__;

            internal int blepType;
            internal int blepPulseWidth;

            internal int additiveSub;
            internal int additiveStep;
            internal int additiveRolloff;
            internal int additivePartials;

            internal int fmCarrier;
            internal int fmModulator;
            internal int fmIndex;
            internal int fmDamping;

            internal TargetModsModel.Native mods;
        }

        static readonly string[] BlepTypeNames = new[] { "Saw", "Pulse", "Tri " };
        static readonly string[] PMTypeNames = new[] { "Sin", "Saw", "Sqr", "Tri " };
        static readonly string[] UnitTypeNames = new[] { "Sine", "Add", "Blep", "FM" };
        static readonly string[] NoteNames = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        static readonly string[] ModTargetNames = new[] { "Amp", "Pan", "Phase", "Pitch", "Freq", "Idx", "Dmp", "PW", "Roll" };

        static readonly IRelevance RelevanceFM = Relevance.Param((UnitModel m) => m.Type, (UnitType t) => t == UnitType.FM);
        static readonly IRelevance RelevanceBlep = Relevance.Param((UnitModel m) => m.Type, (UnitType t) => t == UnitType.PolyBlep);
        static readonly IRelevance RelevanceAdditive = Relevance.Param((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Additive);
        static readonly IRelevance RelevancePulseWidth = Relevance.All(
            Relevance.Param((UnitModel m) => m.Type, (UnitType t) => t == UnitType.PolyBlep),
            Relevance.Param((UnitModel m) => m.BlepType, (BlepType t) => t != Synth0.Model.BlepType.Saw));

        public Param On { get; } = new(OnInfo);
        public Param Amp { get; } = new(AmpInfo);
        public Param Type { get; } = new(TypeInfo);
        public Param Pan { get; } = new(PanInfo);
        static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, 1, nameof(On), "On", "Enabled", false, false);
        static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, 1, nameof(Amp), "Amp", "Amplitude", true, 255);
        static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, 1, nameof(Pan), "Pan", "Panning", true);
        static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, 0, nameof(Type), "Type", "Type", false, UnitTypeNames);

        public Param Note { get; } = new(NoteInfo);
        public Param Octave { get; } = new(OctaveInfo);
        public Param Detune { get; } = new(DetuneInfo);
        static readonly ParamInfo DetuneInfo = ParamInfo.Mix(p => &((Native*)p)->detune, 1, nameof(Detune), "Dtn", "Detune", true);
        static readonly ParamInfo OctaveInfo = ParamInfo.Select(p => &((Native*)p)->octave, 1, nameof(Octave), "Oct", "Octave", true, 0, 9, 4);
        static readonly ParamInfo NoteInfo = ParamInfo.Select<NoteType>(p => &((Native*)p)->note, 1, nameof(Note), nameof(Note), "Note", true, NoteNames);

        public Param BlepType { get; } = new(BlepTypeInfo);
        public Param BlepPulseWidth { get; } = new(BlepPulseWidthInfo);
        static readonly ParamInfo BlepTypeInfo = ParamInfo.List<BlepType>(p => &((Native*)p)->blepType, 0, nameof(BlepType), "Type", "Blep type", false, BlepTypeNames, RelevanceBlep);
        static readonly ParamInfo BlepPulseWidthInfo = ParamInfo.Level(p => &((Native*)p)->blepPulseWidth, 0, nameof(BlepPulseWidth), "PW", "Pulse width", true, 0, RelevancePulseWidth);

        public Param AdditiveSub { get; } = new(AdditiveSubInfo);
        public Param AdditiveStep { get; } = new(AdditiveStepInfo);
        public Param AdditiveRolloff { get; } = new(AdditiveRolloffInfo);
        public Param AdditivePartials { get; } = new(AdditivePartialsInfo);
        static readonly ParamInfo AdditiveSubInfo = ParamInfo.Toggle(p => &((Native*)p)->additiveSub, 0, nameof(AdditiveSub), "Sub", "Additive subtract", true, false, RelevanceAdditive);
        static readonly ParamInfo AdditiveRolloffInfo = ParamInfo.Mix(p => &((Native*)p)->additiveRolloff, 0, nameof(AdditiveRolloff), "Roll", "Additive rolloff", true, RelevanceAdditive);
        static readonly ParamInfo AdditiveStepInfo = ParamInfo.Select(p => &((Native*)p)->additiveStep, 0, nameof(AdditiveStep), "Step", "Additive step", true, 1, 32, 1, RelevanceAdditive);
        static readonly ParamInfo AdditivePartialsInfo = ParamInfo.Select(p => &((Native*)p)->additivePartials, 0, nameof(AdditivePartials), "Parts", "Additive partials", true, 1, 32, 1, RelevanceAdditive);

        public Param FMIndex { get; } = new(FMIndexInfo);
        public Param FMDamping { get; } = new(FMDampingInfo);
        public Param FMCarrier { get; } = new(FMCarrierInfo);
        public Param FMModulator { get; } = new(FMModulatorInfo);
        static readonly ParamInfo FMIndexInfo = ParamInfo.Level(p => &((Native*)p)->fmIndex, 0, nameof(FMIndex), "Idx", "FM index", true, 0, RelevanceFM);
        static readonly ParamInfo FMDampingInfo = ParamInfo.Level(p => &((Native*)p)->fmDamping, 0, nameof(FMDampingInfo), "Dmp", "FM damping", true, 0, RelevanceFM);
        static readonly ParamInfo FMCarrierInfo = ParamInfo.List<FMType>(p => &((Native*)p)->fmCarrier, 0, nameof(FMCarrier), "Car", "FM carrier", true, PMTypeNames, RelevanceFM);
        static readonly ParamInfo FMModulatorInfo = ParamInfo.List<FMType>(p => &((Native*)p)->fmModulator, 0, nameof(FMModulator), "Mod", "FM modulator", true, PMTypeNames, RelevanceFM);

        public Param Mod1Source { get; } = new(Mod1SourceInfo);
        public Param Mod1Target { get; } = new(Mod1TargetInfo);
        public Param Mod1Amount { get; } = new(Mod1AmountInfo);
        static readonly ParamInfo Mod1AmountInfo = ParamInfo.Mix(p => &((Native*)p)->mods.mod1.mod.amount, 2, nameof(Mod1Amount), "Amt", "Mod 1 amount", true);
        static readonly ParamInfo Mod1TargetInfo = ParamInfo.List<UnitModTarget>(p => &((Native*)p)->mods.mod1.target, 2, nameof(Mod1Target), "Tgt", "Mod 1 target", true, ModTargetNames);
        static readonly ParamInfo Mod1SourceInfo = ParamInfo.List<VoiceModSource>(p => &((Native*)p)->mods.mod1.mod.source, 2, nameof(Mod1Source), "Src", "Mod 1 source", true, VoiceModModel.ModSourceNames);

        public Param Mod2Source { get; } = new(Mod2SourceInfo);
        public Param Mod2Target { get; } = new(Mod2TargetInfo);
        public Param Mod2Amount { get; } = new(Mod2AmountInfo);
        static readonly ParamInfo Mod2AmountInfo = ParamInfo.Mix(p => &((Native*)p)->mods.mod2.mod.amount, 2, nameof(Mod2Amount), "Amt", "Mod 2 amount", true);
        static readonly ParamInfo Mod2TargetInfo = ParamInfo.List<UnitModTarget>(p => &((Native*)p)->mods.mod2.target, 2, nameof(Mod2Target), "Tgt", "Mod 2 target", true, ModTargetNames);
        static readonly ParamInfo Mod2SourceInfo = ParamInfo.List<VoiceModSource>(p => &((Native*)p)->mods.mod2.mod.source, 2, nameof(Mod2Source), "Src", "Mod 2 source", true, VoiceModModel.ModSourceNames);
    }
}