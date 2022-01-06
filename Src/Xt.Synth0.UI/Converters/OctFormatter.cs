using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class OctFormatter : MultiConverter<int, int, string>
	{
		readonly PatternKey _model;
		internal OctFormatter(PatternKey model) => _model = model;

		protected override string Convert(int note, int oct)
		{
			if (note >= (int)PatternNote.C) return _model.Octave.Info.Format(oct);
			return new string(_model.Note.Info.Format(note)[0], 1);
		}
	}
}