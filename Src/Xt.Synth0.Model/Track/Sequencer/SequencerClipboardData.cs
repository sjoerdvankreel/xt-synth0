namespace Xt.Synth0.Model
{
    public enum SequencerClipboardType
    {
        Fx,
        Keys,
        Rows
    }

    public class SequencerClipboardData
    {
        public PatternFxModel[] Fx { get; }
        public PatternKeyModel[] Keys { get; }
        public PatternRowModel[] Rows { get; }
        public SequencerClipboardType Type { get; }

        public SequencerClipboardData(PatternFxModel[] fx) => (Type, Fx) = (SequencerClipboardType.Fx, fx);
        public SequencerClipboardData(PatternKeyModel[] keys) => (Type, Keys) = (SequencerClipboardType.Keys, keys);
        public SequencerClipboardData(PatternRowModel[] rows) => (Type, Rows) = (SequencerClipboardType.Rows, rows);
    }
}