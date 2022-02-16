using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class PatternKey : IParamGroupModel
	{
		static readonly string[] Notes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		[StructLayout(LayoutKind.Sequential, Pack = 8)]
		internal ref struct Native
		{
			internal const int Size = 16;
			internal int note, amp, oct, pad__;
		}

		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		internal PatternKey(int index) => Index = index;

		public int Index { get; }
		public string Id => "9C964DBC-691C-494C-A58F-8562CF3A33F4";
		public IReadOnlyList<Param> Params => new[] { Note, Oct, Amp };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->keys[Index * Native.Size];

		static readonly ParamInfo OctInfo = ParamInfo.Pattern(p => &((Native*)p)->oct, nameof(Oct), nameof(Oct), "Octave", 0, 9, 4);
		static readonly ParamInfo NoteInfo = ParamInfo.Pattern(p => &((Native*)p)->note, nameof(Note), nameof(Note), "Note", Notes);
		static readonly ParamInfo AmpInfo = ParamInfo.Pattern(p => &((Native*)p)->amp, nameof(Amp), nameof(Amp), "Amplitude", 0, 255, 255);
	}
}