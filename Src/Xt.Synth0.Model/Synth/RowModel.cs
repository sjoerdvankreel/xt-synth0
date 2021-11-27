namespace Xt.Synth0.Model
{
	public sealed class RowModel
	{
		public static readonly string[] Notes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		static readonly ParamInfo AmpInfo = new ContinuousInfo(nameof(Amp), 255);
		static readonly ParamInfo OctInfo = new DiscreteInfo(nameof(Oct), 0, 9, 4);
		static readonly ParamInfo NoteInfo = new EnumInfo<RowNote>(nameof(Note), Notes);

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