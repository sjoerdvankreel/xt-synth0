using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum WaveType { Saw, Pulse, Tri }
	public enum UnitType { Sin, Add, Blep, Naive }
	public enum ModSource { Off, Env1, Env2, Env3, LFO1, LFO2 }
	public enum ModTarget { Off, Pw, Amp, Pan, Dtn, Roll, Pitch, Phase };
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }
	public enum AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

	public unsafe sealed class UnitModel : IUIParamGroupModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 80;
			internal int on, type, note, addType, waveType;
			internal int amp, pan, oct, dtn, pw;
			internal int src1, tgt1, amt1, src2, tgt2, amt2;
			internal int addMaxParts, addParts, addStep, addRoll;
		}

		public Param On { get; } = new(OnInfo);
		public Param Pw { get; } = new(PwInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Pan { get; } = new(PanInfo);
		public Param Dtn { get; } = new(DtnInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Src1 { get; } = new(Src1Info);
		public Param Tgt1 { get; } = new(Tgt1Info);
		public Param Amt1 { get; } = new(Amt1Info);
		public Param Src2 { get; } = new(Src2Info);
		public Param Tgt2 { get; } = new(Tgt2Info);
		public Param Amt2 { get; } = new(Amt2Info);
		public Param WaveType { get; } = new(WaveTypeInfo);
		public Param AddType { get; } = new(AddTypeInfo);
		public Param AddStep { get; } = new(AddStepInfo);
		public Param AddRoll { get; } = new(AddRollInfo);
		public Param AddParts { get; } = new(AddPartsInfo);
		public Param AddMaxParts { get; } = new(AddMaxPartsInfo);

		public int Columns => 3;
		public int Index { get; }
		public Param Enabled => On;
		public string Name => $"Unit {Index + 1}";
		public ThemeGroup ThemeGroup => ThemeGroup.Unit;
		public string Id => "3DACD0A4-9688-4FA9-9CA3-B8A0E49A45E5";
		public IReadOnlyList<Param> Params => Layout.Keys.ToArray();
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[Index * Native.Size];
		public IDictionary<Param, int> Layout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { AddType, 1 }, { WaveType, 1 },
			{ Amp, 3 }, { Pan, 4 }, { Pw, 5 },
			{ Oct, 6 }, { Note, 7 }, { Dtn, 8 },
			{ AddParts, 9 }, { AddMaxParts, 9 }, { AddStep, 10 }, { AddRoll, 11 },
			{ Src1, 12 }, { Tgt1, 13 }, {Amt1, 14 },
			{ Src2, 15 }, { Tgt2, 16}, {Amt2, 17 },
		};

		internal UnitModel(int index) => Index = index;
		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly string[] AddNames = { "Saw", "Sqr", "Pulse", "Tri", "Impulse", "Sin+Sin", "Sin+Cos", "Sin-Sin", "Sin-Cos" };
		static readonly AddType[] CustomAddTypes = new[] { Synth0.Model.AddType.SinAddSin, Synth0.Model.AddType.SinSubSin, Synth0.Model.AddType.SinAddCos, Synth0.Model.AddType.SinSubCos };
		static readonly AddType[] BasicAddTypes = new[] { Synth0.Model.AddType.Saw, Synth0.Model.AddType.Sqr, Synth0.Model.AddType.Pulse, Synth0.Model.AddType.Tri, Synth0.Model.AddType.Impulse };

		static readonly IRelevance RelevanceWave = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Naive);
		static readonly IRelevance RelevanceAdd = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add);
		static readonly IRelevance RelevanceAddBasic = Relevance.All(RelevanceAdd,
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => BasicAddTypes.Contains(t)));
		static readonly IRelevance RelevanceAddCustom = Relevance.All(RelevanceAdd,
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => CustomAddTypes.Contains(t)));
		static readonly IRelevance RelevancePw = Relevance.Any(
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Naive),
			Relevance.When((UnitModel m) => m.WaveType, (WaveType t) => t == Synth0.Model.WaveType.Pulse)),
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add),
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => t == Synth0.Model.AddType.Pulse)));

		static readonly ParamInfo DtnInfo = ParamInfo.Mix(p => &((Native*)p)->dtn, nameof(Dtn), nameof(Dtn), "Detune");
		static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, nameof(Pan), nameof(Pan), "Panning");
		static readonly ParamInfo Amt1Info = ParamInfo.Level(p => &((Native*)p)->amt1, nameof(Amt1), "Amt", "Mod 1 amount", 0);
		static readonly ParamInfo Amt2Info = ParamInfo.Level(p => &((Native*)p)->amt2, nameof(Amt2), "Amt", "Mod 2 amount", 0);
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), nameof(On), "Enabled", false);
		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), nameof(Amp), "Amplitude", 255);
		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, nameof(Oct), nameof(Oct), "Octave", 0, 9, 4);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type), nameof(Type), "Type");
		static readonly ParamInfo PwInfo = ParamInfo.Mix(p => &((Native*)p)->pw, nameof(Pw), "PW", "Pulse width", null, RelevancePw);
		static readonly ParamInfo Src1Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src1, nameof(Src1), "Source", "Mod 1 source");
		static readonly ParamInfo Tgt1Info = ParamInfo.List<ModTarget>(p => &((Native*)p)->tgt1, nameof(Tgt1), "Target", "Mod 1 target");
		static readonly ParamInfo Src2Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src2, nameof(Src2), "Source", "Mod 2 source");
		static readonly ParamInfo Tgt2Info = ParamInfo.List<ModTarget>(p => &((Native*)p)->tgt2, nameof(Tgt2), "Target", "Mod 2 target");
		static readonly ParamInfo NoteInfo = ParamInfo.Select(p => &((Native*)p)->note, nameof(Note), nameof(Note), "Note", UnitNote.C, UnitNote.C, Notes);
		static readonly ParamInfo WaveTypeInfo = ParamInfo.List<WaveType>(p => &((Native*)p)->waveType, nameof(WaveType), "Type", "Wave type", null, RelevanceWave);
		static readonly ParamInfo AddTypeInfo = ParamInfo.List<AddType>(p => &((Native*)p)->addType, nameof(AddType), "Type", "Additive type", AddNames, RelevanceAdd);
		static readonly ParamInfo AddRollInfo = ParamInfo.Mix(p => &((Native*)p)->addRoll, nameof(AddRoll), "Roll", "Additive custom rolloff", null, RelevanceAddCustom);
		static readonly ParamInfo AddMaxPartsInfo = ParamInfo.Exp(p => &((Native*)p)->addMaxParts, nameof(AddMaxParts), "Hms", "Additive basic partials", 12, 4, RelevanceAddBasic);
		static readonly ParamInfo AddStepInfo = ParamInfo.Select(p => &((Native*)p)->addStep, nameof(AddStep), "Step", "Additive custom step", 1, 32, 1, null, RelevanceAddCustom);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Select(p => &((Native*)p)->addParts, nameof(AddParts), "Hms", "Additive custom partials", 1, 32, 1, null, RelevanceAddCustom);
	}
}