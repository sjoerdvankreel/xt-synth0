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

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal struct Native {
			internal const int Size = 16;
			internal int note, amp, oct, pad__; 
		}

		public Param Amp { get; } = new(AmpInfo);
		public Param Note { get; } = new(NoteInfo);
		public Param Oct { get; } = new(OctInfo);

		readonly int _index;
		internal PatternKey(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Note, Oct, Amp };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->keys[_index * Native.Size];

		static readonly ParamInfo AmpInfo = ParamInfo.Level(p => &((Native*)p)->amp, nameof(Amp), "Amplitude", false, 255);
		static readonly ParamInfo OctInfo = ParamInfo.Select(p => &((Native*)p)->oct, nameof(Oct), "Octave", false, 0, 9, 4);
		static readonly ParamInfo NoteInfo = ParamInfo.List<PatternNote>(p => &((Native*)p)->note, nameof(Note), "Note", false, Notes);
	}
}