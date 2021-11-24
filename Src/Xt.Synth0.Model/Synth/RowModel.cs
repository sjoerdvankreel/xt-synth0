namespace Xt.Synth0.Model
{
	public class RowModel
	{
		public const int NoteOff = NoteModel.NoteCount;
		public const int NoNote = NoteModel.NoteCount + 1;

		static readonly ParamInfo OctInfo = new(ParamType.Int, nameof(Oct), 0, 9, 4);
		static readonly ParamInfo AmpInfo = new(ParamType.Int, nameof(Amp), 0, 255, 255);
		static readonly ParamInfo NoteInfo = new(ParamType.Note, nameof(Note), 0, NoNote, NoNote);

		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);
		internal Param[] Params() => new[] { Amp, Note, Oct };

		internal void CopyTo(RowModel model)
		{
			model.Amp.Value = Amp.Value;
			model.Note.Value = Note.Value;
			model.Oct.Value = Oct.Value;
		}
	}
}