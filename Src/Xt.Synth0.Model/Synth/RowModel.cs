namespace Xt.Synth0.Model
{
	public class RowModel
	{
		public const int NoteOff = NoteModel.NoteCount;
		public const int NoNote = NoteModel.NoteCount + 1;

		static readonly ParamInfo<int> AmpInfo = new(ParamType.Int, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo<int> OctaveInfo = new(ParamType.Int, nameof(Octave), 0, 9, 4);
		static readonly ParamInfo<int> NoteInfo = new(ParamType.Note, nameof(Note), 0, NoNote, NoNote);

		public Param<int> Amp { get; } = Param.Of(AmpInfo);
		public Param<int> Note { get; } = Param.Of(NoteInfo);
		public Param<int> Octave { get; } = Param.Of(OctaveInfo);

		internal Param<int>[] IntParams() => new[] { Amp, Note, Octave };

		internal void CopyTo(RowModel model)
		{
			model.Amp.Value = Amp.Value;
			model.Note.Value = Note.Value;
			model.Octave.Value = Octave.Value;
		}
	}
}