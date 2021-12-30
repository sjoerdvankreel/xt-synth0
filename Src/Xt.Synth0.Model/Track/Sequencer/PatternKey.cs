using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
	public enum PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

	public unsafe sealed class PatternKey : ISubModel
	{
		static PatternKey()
		{
			if (Size != XtsPatternKeySize())
				throw new InvalidOperationException();
		}

		public static readonly string[] Notes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		internal const int Size = 1;
		[DllImport("Xt.Synth0.DSP.Native")]
		static extern int XtsPatternKeySize();
		[StructLayout(LayoutKind.Sequential)]
		internal struct Native { internal int amp, oct, note; }

		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);

		readonly int _index;
		internal PatternKey(int index) => _index = index;
		public IReadOnlyList<Param> Params => new[] { Note, Oct, Amp };
		public void* Address(void* parent) => &((PatternRow.Native*)parent)->keys[_index * Size];

		static readonly ParamInfo OctInfo = new DiscreteInfo(p => &((Native*)p)->oct, nameof(Oct), "Octave", 0, 9, 4);
		static readonly ParamInfo AmpInfo = new ContinuousInfo(p => &((Native*)p)->amp, nameof(Amp), "Velocity", 255);
		static readonly ParamInfo NoteInfo = new EnumInfo<PatternNote>(p => &((Native*)p)->note, nameof(Note), "Note", Notes);
	}
}