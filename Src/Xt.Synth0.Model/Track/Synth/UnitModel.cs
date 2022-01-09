using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitWave { Saw, Pulse, Tri }
	public enum UnitType { Off, Sin, Naive, BasicAdd, Additive }
	public enum AdditiveType { SinPlusSin, SinPlusCos, SinMinSin, SinMinCos };
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class UnitModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int type, wave, amp, oct, note, cent;
			internal int basicAddLogParts;
			internal int addType, addParts, addStep, addRolloff;
			internal int pad__;
		}

		public Param Oct { get; } = new(OctInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Wave { get; } = new(WaveInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param AddType { get; } = new(AddTypeInfo);
		public Param AddStep { get; } = new(AddStepInfo);
		public Param AddParts { get; } = new(AddPartsInfo);
		public Param AddRolloff { get; } = new(AddRolloffInfo);
		public Param BasicAddLogParts { get; } = new(BasicAddLogPartsInfo);

		readonly int _index;
		public string Name => $"Unit {_index + 1}";
		internal UnitModel(int index) => _index = index;
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[_index * TrackConstants.UnitModelSize];

		public IDictionary<Param, int> ParamLayout => new Dictionary<Param, int>
		{
			{ Type, 0 },
			{ Wave, 1 },
			{ Amp, 2 },
			{ Oct, 3 },
			{ Note, 4 },
			{ Cent, 5 },
			{ BasicAddLogParts, 6 },
			{ AddType, 6 },
			{ AddParts, 7 },
			{ AddStep, 8 },
			{ AddRolloff, 9 },
		};

		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly ParamInfo NoteInfo = ParamInfo.Lin(p => &((Native*)p)->note, nameof(Note), Notes);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type));
		static readonly ParamInfo CentInfo = ParamInfo.Lin(p => &((Native*)p)->cent, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo OctInfo = ParamInfo.Lin(p => &((Native*)p)->oct, nameof(Oct), TrackConstants.MinOct, TrackConstants.MaxOct, 4);
		static readonly ParamInfo AddStepInfo = ParamInfo.Lin(p => &((Native*)p)->addStep, "Step", 1, 32, 1, null, m => ((UnitModel)m).Type, (int)UnitType.Additive);
		static readonly ParamInfo AddPartsInfo = ParamInfo.Lin(p => &((Native*)p)->addParts, "Parts", 1, 32, 1, null, m => ((UnitModel)m).Type, (int)UnitType.Additive);
		static readonly ParamInfo AddTypeInfo = ParamInfo.List<AdditiveType>(p => &((Native*)p)->addType, "Type", null, m => ((UnitModel)m).Type, (int)UnitType.Additive);
		static readonly ParamInfo AddRolloffInfo = ParamInfo.Lin(p => &((Native*)p)->addRolloff, "Rolloff", 0, 255, 0, null, m => ((UnitModel)m).Type, (int)UnitType.Additive);
		static readonly ParamInfo BasicAddLogPartsInfo = ParamInfo.Exp(p => &((Native*)p)->basicAddLogParts, "Parts", 0, 10, 4, m => ((UnitModel)m).Type, (int)UnitType.BasicAdd);
		static readonly ParamInfo WaveInfo = ParamInfo.List<UnitWave>(p => &((Native*)p)->wave, nameof(Wave), null, m => ((UnitModel)m).Type, (int)UnitType.Naive, (int)UnitType.BasicAdd);
	}
}