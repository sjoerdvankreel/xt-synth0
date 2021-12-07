using System.Collections.Generic;

namespace Xt.Synth0.Model
{
	public sealed class PatternKey : SubModel
	{
		const string NoteDetail = "Note";
		const string OctDetail = "Octave";
		const string AmpDetail = "Velocity";

		public static readonly string[] Notes = new[] {
			"..", "==", "C-", "C#", "D-", "D#", "E-",
			"F-", "F#", "G-", "G#", "A-", "A#", "B-"
		};

		static readonly ParamInfo OctInfo = new DiscreteInfo(
			nameof(Oct), OctDetail, 0, 9, 4);
		static readonly ParamInfo AmpInfo = new ContinuousInfo(
			nameof(Amp), AmpDetail, 255);
		static readonly ParamInfo NoteInfo = new EnumInfo<PatternNote>(
			nameof(Note), NoteDetail, Notes);

		public Param Amp { get; } = new(AmpInfo);
		public Param Oct { get; } = new(OctInfo);
		public Param Note { get; } = new(NoteInfo);

		internal override IEnumerable<Param> ListParams() 
		=> new[] { Amp, Oct, Note };
	}
}