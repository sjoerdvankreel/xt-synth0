using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum UnitWave { Sin, Saw, Sqr, Tri }
	public enum UnitType { PolyBlep, Additive, Naive }
	public enum UnitNote { C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class UnitModel : INamedModel
	{
		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int on, type, wave, partialCount, amp, octave, note, cent; }

		public Param On { get; } = new(OnInfo);
		public Param Amp { get; } = new(AmpInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Cent { get; } = new(CentInfo);
		public Param Wave { get; } = new(WaveInfo);
		public Param Type { get; } = new(TypeInfo);
		public Param Octave { get; } = new(OctaveInfo);
		public Param PartialCount { get; } = new(PartialCountInfo);

		readonly int _index;
		public string Name => $"Unit {_index + 1}";
		internal UnitModel(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { On, Type, Wave, PartialCount, Amp, Octave, Note, Cent };
		public void* Address(void* parent) => &((SynthModel.Native*)parent)->units[_index * TrackConstants.UnitModelSize];

		static readonly string[] Notes = new[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
		static readonly ParamInfo OnInfo = ParamInfo.Toggle(p => &((Native*)p)->on, nameof(On), false);
		static readonly ParamInfo NoteInfo = ParamInfo.Lin(p => &((Native*)p)->note, nameof(Note), Notes);
		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo TypeInfo = ParamInfo.List<UnitType>(p => &((Native*)p)->type, nameof(Type));
		static readonly ParamInfo WaveInfo = ParamInfo.List<UnitWave>(p => &((Native*)p)->wave, nameof(Wave));
		static readonly ParamInfo CentInfo = ParamInfo.Lin(p => &((Native*)p)->cent, nameof(Cent), -50, 49, 0);
		static readonly ParamInfo PartialCountInfo = ParamInfo.Exp(p => &((Native*)p)->hmns, nameof(PartialCount), 0, 10, 4);
		static readonly ParamInfo OctaveInfo = ParamInfo.Lin(p => &((Native*)p)->oct, nameof(Octave), TrackConstants.MinOctave, TrackConstants.MaxOctave, 4);
	}
}