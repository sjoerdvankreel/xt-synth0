using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitType { Sin, Naive, Add }
	public enum NaiveType { Saw, Pulse, Tri }
	public enum ModSource { Off, Env1, Env2, Env3, Lfo1, Lfo2 }
	public enum ModTarget { Off, Pitch, Amp, Pan, Dtn, Pw, Roll };
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }
	public enum AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

	public unsafe sealed class UnitModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native
		{
			internal const int Size = 80;
			internal int on, type, note, addType, naiveType;
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
		public Param NaiveType { get; } = new(NaiveTypeInfo);
		public Param AddType { get; } = new(AddTypeInfo);
		public Param AddStep { get; } = new(AddStepInfo);
		public Param AddRoll { get; } = new(AddRollInfo);
		public Param AddParts { get; } = new(AddPartsInfo);
		public Param AddMaxParts { get; } = new(AddMaxPartsInfo);

		readonly int _index;
		public string Name => $"Unit {_index + 1}";
		public ThemeGroup Group => ThemeGroup.Units;
		internal UnitModel(int index) => _index = index;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[_index * Native.Size];

		public Param Enabled => On;
		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ On, -1 },
			{ Type, 0 }, { AddType, 1 }, { NaiveType, 1 },
			{ Amp, 3 }, { Pan, 4 }, { Pw, 5 },
			{ Oct, 6 }, { Note, 7 }, { Dtn, 8 },
			{ AddParts, 9 }, { AddMaxParts, 9 }, { AddStep, 10 }, { AddRoll, 11 },
			{ Src1, 12 }, { Tgt1, 13 }, {Amt1, 14 },
			{ Src2, 15 }, { Tgt2, 16}, {Amt2, 17 },
		};

		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly string[] AddNames = { "Saw", "Sqr", "Pulse", "Tri", "Impulse", "Sin+Sin", "Sin+Cos", "Sin-Sin", "Sin-Cos" };
		static readonly AddType[] CustomAddTypes = new[] { Synth0.Model.AddType.SinAddSin, Synth0.Model.AddType.SinSubSin, Synth0.Model.AddType.SinAddCos, Synth0.Model.AddType.SinSubCos };
		static readonly AddType[] BasicAddTypes = new[] { Synth0.Model.AddType.Saw, Synth0.Model.AddType.Sqr, Synth0.Model.AddType.Pulse, Synth0.Model.AddType.Tri, Synth0.Model.AddType.Impulse };

		static readonly IRelevance RelevanceNaive = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Naive);
		static readonly IRelevance RelevanceAdd = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add);
		static readonly IRelevance RelevanceAddBasic = Relevance.All(RelevanceAdd,
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => BasicAddTypes.Contains(t)));
		static readonly IRelevance RelevanceAddCustom = Relevance.All(RelevanceAdd,
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => CustomAddTypes.Contains(t)));
		static readonly IRelevance RelevancePw = Relevance.Any(
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Naive),
			Relevance.When((UnitModel m) => m.NaiveType, (NaiveType t) => t == Synth0.Model.NaiveType.Pulse)),
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add),
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => t == Synth0.Model.AddType.Pulse)));

		static readonly ParamInfo DtnInfo = ParamInfo.Mix(p => &((Native*)p)->dtn, nameof(Dtn), "Detune", true);
		static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, nameof(Pan), "Panning", true);
		static readonly ParamInfo Amt1Info = ParamInfo.Level(p => &((Native*)p)->amt1, "Amt", "Mod 1 amount", true, 0);
		static readonly ParamInfo Amt2Info = ParamInfo.Level(p => &((Native*)p)->amt2, "Amt", "Mod 2 amount", true, 0);
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), "Enabled", false, false);
		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 255);
		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, nameof(Oct), "Octave", true, 0, 9, 4);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type), "Type", true);
		static readonly ParamInfo PwInfo = ParamInfo.Mix(p => &((Native*)p)->pw, "PW", "Pulse width", true, null, RelevancePw);
		static readonly ParamInfo Src1Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src1, "Source", "Mod 1 source", true);
		static readonly ParamInfo Tgt1Info = ParamInfo.List<ModTarget>(p => &((Native*)p)->tgt1, "Target", "Mod 1 target", true);
		static readonly ParamInfo Src2Info = ParamInfo.List<ModSource>(p => &((Native*)p)->src2, "Source", "Mod 2 source", true);
		static readonly ParamInfo Tgt2Info = ParamInfo.List<ModTarget>(p => &((Native*)p)->tgt2, "Target", "Mod 2 target", true);
		static readonly ParamInfo NoteInfo = ParamInfo.Select(p => &((Native*)p)->note, nameof(Note), "Note", true, UnitNote.C, Notes);
		static readonly ParamInfo AddTypeInfo = ParamInfo.List<AddType>(p => &((Native*)p)->addType, "Type", "Additive type", true, AddNames, RelevanceAdd);
		static readonly ParamInfo NaiveTypeInfo = ParamInfo.List<NaiveType>(p => &((Native*)p)->naiveType, "Type", "Naive type", true, null, RelevanceNaive);
		static readonly ParamInfo AddRollInfo = ParamInfo.Mix(p => &((Native*)p)->addRoll, "Roll", "Additive custom rolloff", true, null, RelevanceAddCustom);
		static readonly ParamInfo AddMaxPartsInfo = ParamInfo.Exp(p => &((Native*)p)->addMaxParts, "Hms", "Additive basic partials", true, 12, 4, RelevanceAddBasic);
		static readonly ParamInfo AddStepInfo = ParamInfo.Select(p => &((Native*)p)->addStep, "Step", "Additive custom step", true, 1, 32, 1, null, RelevanceAddCustom);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Select(p => &((Native*)p)->addParts, "Hms", "Additive custom partials", true, 1, 32, 1, null, RelevanceAddCustom);
	}
}