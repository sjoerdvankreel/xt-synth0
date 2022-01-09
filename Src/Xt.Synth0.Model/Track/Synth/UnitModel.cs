using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitType { Off, Sine, Naive, Additive }
	public enum NaiveType { Saw, Pulse, Triangle, Impulse }
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }
	public enum AdditiveType { Saw, Pulse, Triangle, Impulse, SinAddSin, SinAddCos, SinSubSin, SinSubCos };

	public unsafe sealed class UnitModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int type, naiveType, amp, oct, note, cent, pwm;
			internal int addType, addParts, addMaxParts, addStep, addRolloff;
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
		public Param AddParts { get; } = new(AddPartsInfo);
		public Param AddRolloff { get; } = new(AddRolloffInfo);
		public Param AddMaxParts { get; } = new(AddMaxPartsInfo);

		readonly int _index;
		public string Name => $"Unit {_index + 1}";
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
			{ AddRolloff, 9 },
		};

		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly AdditiveType[] BasicAddTypes = new[] { AdditiveType.Saw, AdditiveType.Pulse, AdditiveType.Triangle, AdditiveType.Impulse };
		static readonly AdditiveType[] CustomAddTypes = new[] { AdditiveType.SinAddSin, AdditiveType.SinSubSin, AdditiveType.SinAddCos, AdditiveType.SinSubCos };

		static readonly IRelevance RelevanceNaive = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Naive);
		static readonly IRelevance RelevanceAdditive = Relevance.When(
			(UnitModel m) => m.Type, (UnitType t) => t == UnitType.Additive);
		static readonly IRelevance RelevanceAddBasic = Relevance.All(RelevanceAdditive,
			Relevance.When((UnitModel m) => m.AddType, (AdditiveType t) => BasicAddTypes.Contains(t)));
		static readonly IRelevance RelevanceAddCustom = Relevance.All(RelevanceAdditive,
			Relevance.When((UnitModel m) => m.AddType, (AdditiveType t) => CustomAddTypes.Contains(t)));

		static readonly ParamInfo PwmInfo = ParamInfo.Lin(p => &((Native*)p)->pwm, "PWM", 0, 255, 0);
		static readonly ParamInfo NoteInfo = ParamInfo.Lin(p => &((Native*)p)->note, nameof(Note), Notes);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type));
		static readonly ParamInfo CentInfo = ParamInfo.Lin(p => &((Native*)p)->cent, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo OctInfo = ParamInfo.Lin(p => &((Native*)p)->oct, nameof(Oct), TrackConstants.MinOct, TrackConstants.MaxOct, 4);
		static readonly ParamInfo NaiveTypeInfo = ParamInfo.List<NaiveType>(p => &((Native*)p)->naiveType, "Type", null, RelevanceNaive);
		static readonly ParamInfo AddTypeInfo = ParamInfo.List<AdditiveType>(p => &((Native*)p)->addType, "Type", null, RelevanceAdditive);
		static readonly ParamInfo AddMaxPartsInfo = ParamInfo.Exp(p => &((Native*)p)->addMaxParts, "Parts", 0, 12, 4, RelevanceAddBasic);
		static readonly ParamInfo AddStepInfo = ParamInfo.Lin(p => &((Native*)p)->addStep, "Step", 1, 32, 1, null, RelevanceAddCustom);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Lin(p => &((Native*)p)->addParts, "Parts", 1, 32, 1, null, RelevanceAddCustom);
		static readonly ParamInfo AddRolloffInfo = ParamInfo.Lin(p => &((Native*)p)->addRolloff, "Rolloff", 0, 255, 0, null, RelevanceAddCustom);
	}
}