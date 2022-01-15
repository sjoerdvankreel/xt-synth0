using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum NaiveType { Saw, Pulse, Tri }
	public enum UnitType { Off, Sin, Naive, Add }
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }
	public enum AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

	public unsafe sealed class UnitModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int type, naiveType, amp, pan, oct, note, dtn, pw;
			internal int addType, addParts, addMaxParts, addStep, addRoll, pad__;
		}

		public Param Pw { get; } = new(PwInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Pan { get; } = new(PanInfo);
		public Param Dtn { get; } = new(DtnInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Type { get; } = new(TypeInfo);
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
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[_index * TrackConstants.UnitModelSize];

		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ Type, 0 },
			{ AddType, 1 },
			{ NaiveType, 1 },
			{ Amp, 2 },
			{ Pan, 3 },
			{ Oct, 4 },
			{ Note, 5 },
			{ Dtn, 6 },
			{ Pw, 7 },
			{ AddParts, 7 },
			{ AddMaxParts, 8 },
			{ AddStep, 8 },
			{ AddRoll, 9 }
		};

		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly string[] AddNames = { "Saw", "Sqr", "Pulse", "Tri", "Impulse", "Sin+Sin", "Sin+Cos", "Sin-Sin", "Sin-Cos" };
		static readonly AddType[] CustomAddTypes = new[] { Model.AddType.SinAddSin, Model.AddType.SinSubSin, Model.AddType.SinAddCos, Model.AddType.SinSubCos };
		static readonly AddType[] BasicAddTypes = new[] { Model.AddType.Saw, Model.AddType.Sqr, Model.AddType.Pulse, Model.AddType.Tri, Model.AddType.Impulse };

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
			Relevance.When((UnitModel m) => m.NaiveType, (NaiveType t) => t == Model.NaiveType.Pulse)),
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add),
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => t == Model.AddType.Pulse)));

		static readonly ParamInfo DtnInfo = ParamInfo.Mix(p => &((Native*)p)->dtn, nameof(Dtn), "Detune", true);
		static readonly ParamInfo PanInfo = ParamInfo.Mix(p => &((Native*)p)->pan, nameof(Pan), "Panning", true);
		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", true, 255);
		static readonly ParamInfo NoteInfo = ParamInfo.Select(p => &((Native*)p)->note, nameof(Note), "Note", true, Notes);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type), "Type", false);
		static readonly ParamInfo PwInfo = ParamInfo.Mix(p => &((Native*)p)->pw, "PW", "Pulse width", true, null, RelevancePw);
		static readonly ParamInfo AddTypeInfo = ParamInfo.List<AddType>(p => &((Native*)p)->addType, "Type", "Additive type", true, AddNames, RelevanceAdd);
		static readonly ParamInfo NaiveTypeInfo = ParamInfo.List<NaiveType>(p => &((Native*)p)->naiveType, "Type", "Naive type", true, null, RelevanceNaive);
		static readonly ParamInfo AddRollInfo = ParamInfo.Mix(p => &((Native*)p)->addRoll, "Roll", "Additive custom rolloff", true, null, RelevanceAddCustom);
		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, nameof(Oct), "Octave", true, TrackConstants.MinOct, TrackConstants.MaxOct, 4);
		static readonly ParamInfo AddMaxPartsInfo = ParamInfo.Exp(p => &((Native*)p)->addMaxParts, "Hms", "Additive basic partials", true, 12, 4, RelevanceAddBasic);
		static readonly ParamInfo AddStepInfo = ParamInfo.Select(p => &((Native*)p)->addStep, "Step", "Additive custom step", true, 1, 32, 1, null, RelevanceAddCustom);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Select(p => &((Native*)p)->addParts, "Hms", "Additive custom partials", true, 1, 32, 1, null, RelevanceAddCustom);
	}
}