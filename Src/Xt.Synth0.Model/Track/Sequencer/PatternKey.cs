using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class PatternKey : ISubModel
	{
		public static readonly string[] Notes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		[StructLayout(LayoutKind.Sequential, Pack = TrackConstants.Alignment)]
		internal struct Native { internal int amp, note, octave, pad__; }

		public Param Amp { get; } = new(AmpInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Octave { get; } = new(OctaveInfo);

		readonly int _index;
		internal PatternKey(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Note, Octave, Amp };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->keys[_index * TrackConstants.PatternKeySize];

		static readonly ParamInfo AmpInfo = ParamInfo.Lin(p => &((Native*)p)->amp, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo OctaveInfo = ParamInfo.Lin(p => &((Native*)p)->octave, nameof(Octave), 0, 9, 4);
		static readonly ParamInfo NoteInfo = ParamInfo.List<PatternNote>(p => &((Native*)p)->note, nameof(Note), Notes);
	}
}