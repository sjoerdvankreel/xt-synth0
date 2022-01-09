using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitWave { Saw, Pulse, Tri }
	public enum UnitType { Off, Sin, Naive, BasicAdd, CustAdd }
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class UnitModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native
		{
			internal int type, wave, amp, oct, note, cent, basicAddLogParts;
			internal int custAddParts, custAddStep, custAddNegate, custAddQuadRolloff, pad__;
		}

		public Param Oct { get; } = new(OctInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Wave { get; } = new(WaveInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param CustAddStep { get; } = new(CustAddStepInfo);
		public Param CustAddParts { get; } = new(CustAddPartsInfo);
		public Param CustAddNegate { get; } = new(CustAddNegateInfo);
		public Param BasicAddLogParts { get; } = new(BasicAddLogPartsInfo);
		public Param CustAddQuadRolloff { get; } = new(CustAddQuadRolloffInfo);

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
			{ CustAddParts, 6 },
			{ CustAddStep, 7 },
			{ CustAddNegate, 8 },
			{ CustAddQuadRolloff, 9 },
		};

		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly ParamInfo NoteInfo = ParamInfo.Lin(p => &((Native*)p)->note, nameof(Note), Notes);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type));
		static readonly ParamInfo CentInfo = ParamInfo.Lin(p => &((Native*)p)->cent, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo OctInfo = ParamInfo.Lin(p => &((Native*)p)->oct, nameof(Oct), TrackConstants.MinOct, TrackConstants.MaxOct, 4);
		static readonly ParamInfo CustAddStepInfo = ParamInfo.Lin(p => &((Native*)p)->custAddStep, "Step", 1, 16, 1, null, m => ((UnitModel)m).Type, (int)UnitType.CustAdd);
		static readonly ParamInfo CustAddNegateInfo = ParamInfo.Toggle(p => &((Native*)p)->custAddNegate, "Negate", false, m => ((UnitModel)m).Type, (int)UnitType.CustAdd);
		static readonly ParamInfo CustAddPartsInfo = ParamInfo.Lin(p => &((Native*)p)->custAddParts, "Parts", 1, 32, 1, null, m => ((UnitModel)m).Type, (int)UnitType.CustAdd);
		static readonly ParamInfo BasicAddLogPartsInfo = ParamInfo.Exp(p => &((Native*)p)->basicAddLogParts, "Parts", 0, 10, 4, m => ((UnitModel)m).Type, (int)UnitType.BasicAdd);
		static readonly ParamInfo CustAddQuadRolloffInfo = ParamInfo.Toggle(p => &((Native*)p)->custAddQuadRolloff, "Quad rolloff", false, m => ((UnitModel)m).Type, (int)UnitType.CustAdd);
		static readonly ParamInfo WaveInfo = ParamInfo.List<UnitWave>(p => &((Native*)p)->wave, nameof(Wave), null, m => ((UnitModel)m).Type, (int)UnitType.Naive, (int)UnitType.Naive, (int)UnitType.BasicAdd);
	}
}