using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Xt.Synth0.Model
{
    public enum PatternNote { None, Off, C, CSharp, D, DSharp, E, F, FSharp, G, GSharp, A, ASharp, B }

    public unsafe sealed class PatternKeyModel : IParamGroupModel
    {
        public int Index { get; }
        internal PatternKeyModel(int index) => Index = index;

        public string Id => "9C964DBC-691C-494C-A58F-8562CF3A33F4";
        public IReadOnlyList<Param> Params => new[] { Note, Octave, Velocity };
        public void* Address(void* parent) => &((PatternRowModel.Native*)parent)->keys[Index * Native.Size];

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal ref struct Native
        {
            internal const int Size = 16;
            internal int octave;
            internal int velocity;
            internal int note;
            internal int pad__;
        }

        static readonly string[] NoteNames = new[] { "..", "==", "C-", "C#", "D-", "D#", "E-", "F-", "F#", "G-", "G#", "A-", "A#", "B-" };

        public Param Note { get; } = new(NoteInfo);
        public Param Octave { get; } = new(OctaveInfo);
        public Param Velocity { get; } = new(VelocityInfo);
        static readonly ParamInfo NoteInfo = ParamInfo.Pattern(p => &((Native*)p)->note, nameof(Note), "Note", "Note", false, NoteNames);
        static readonly ParamInfo OctaveInfo = ParamInfo.Pattern(p => &((Native*)p)->octave, nameof(Octave), "Octave", "Octave", false, 0, 9, 4);
        static readonly ParamInfo VelocityInfo = ParamInfo.Pattern(p => &((Native*)p)->velocity, nameof(Velocity), "Velocity", "Velocity", false, 0, 255, 255);

        public void Clear()
        {
            Note.Value = 0;
            Octave.Value = 0;
            Velocity.Value = 255;
        }

        public PatternKeyModel Copy()
        {
            var result = new PatternKeyModel(Index);
            CopyTo(result);
            return result;
        }

        public void CopyTo(PatternKeyModel model)
        {
            model.Note.Value = Note.Value;
            model.Octave.Value = Octave.Value;
            model.Velocity.Value = Velocity.Value;
        }
    }
}