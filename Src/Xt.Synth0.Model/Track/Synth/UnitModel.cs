using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitType { Off, Sin, Naive, Add }
	public enum NaiveType { Saw, Pulse, Tri }
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }
	public enum AddType { Saw, Sqr, Pulse, Tri, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

	public unsafe sealed class UnitModel : IThemedSubModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int type, naiveType, amp, oct, note, cent, pwm;
			internal int addType, addParts, addMaxParts, addStep, addRoll;
		}

		public Param Oct { get; } = new(OctInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Pwm { get; } = new(PwmInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
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
			{ Oct, 3 },
			{ Note, 4 },
			{ Cent, 5 },
			{ Pwm, 6 },
			{ AddParts, 7 },
			{ AddMaxParts, 7 },
			{ AddStep, 8 },
			{ AddRoll, 9 },
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
		static readonly IRelevance RelevancePwm = Relevance.Any(
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Naive),
			Relevance.When((UnitModel m) => m.NaiveType, (NaiveType t) => t == Model.NaiveType.Pulse)),
			Relevance.All(Relevance.When((UnitModel m) => m.Type, (UnitType t) => t == UnitType.Add),
			Relevance.When((UnitModel m) => m.AddType, (AddType t) => t == Model.AddType.Pulse)));

		static readonly ParamInfo NoteInfo = ParamInfo.Lin(p => &((Native*)p)->note, nameof(Note), "Note", true, Notes);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), "Level", true, 0, 255, 255);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type), "Type", false);
		static readonly ParamInfo CentInfo = ParamInfo.Lin(p => &((Native*)p)->cent, nameof(Cent), "Detune", true, -50, 49, 0);
		static readonly ParamInfo PwmInfo = ParamInfo.Lin(p => &((Native*)p)->pwm, "PWM", "Pulse width", true, 1, 255, 128, null, RelevancePwm);
		static readonly ParamInfo AddTypeInfo = ParamInfo.List<AddType>(p => &((Native*)p)->addType, "Type", "Additive type", true, AddNames, RelevanceAdd);
		static readonly ParamInfo NaiveTypeInfo = ParamInfo.List<NaiveType>(p => &((Native*)p)->naiveType, "Type", "Naive type", true, null, RelevanceNaive);
		static readonly ParamInfo OctInfo = ParamInfo.Lin(p => &((Native*)p)->oct, nameof(Oct), "Octave", true, TrackConstants.MinOct, TrackConstants.MaxOct, 4);
		static readonly ParamInfo AddStepInfo = ParamInfo.Lin(p => &((Native*)p)->addStep, "Step", "Additive custom wave step", true, 1, 32, 1, null, RelevanceAddCustom);
		static readonly ParamInfo AddRollInfo = ParamInfo.Lin(p => &((Native*)p)->addRoll, "Roll", "Additive custom wave rolloff", true, 0, 255, 0, null, RelevanceAddCustom);
		static readonly ParamInfo AddMaxPartsInfo = ParamInfo.Exp(p => &((Native*)p)->addMaxParts, "Hms", "Additive basic wave harmonic count", true, 0, 12, 4, RelevanceAddBasic);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Lin(p => &((Native*)p)->addParts, "Hms", "Additive custom wave harmonic count", true, 1, 32, 1, null, RelevanceAddCustom);
	}
}