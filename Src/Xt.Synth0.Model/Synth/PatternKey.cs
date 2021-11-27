namespace Xt.Synth0.Model
{
	public sealed class PatternKey: SubModel
	{
		public static readonly string[] Notes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		static readonly ParamInfo AmpInfo = new ContinuousInfo(nameof(Amp), 255);
		static readonly ParamInfo OctInfo = new DiscreteInfo(nameof(Oct), 0, 9, 4);
		static readonly ParamInfo NoteInfo = new EnumInfo<PatternNote>(nameof(Note), Notes);

		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);

		internal override Param[] ListParams() => new[] { Amp, Oct, Note };
	}
}