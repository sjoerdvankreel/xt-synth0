using Xt.Synth0.Model;

namespace Xt.Synth0.UI
{
	class OctFormatter : MultiConverter<int, int, string>
	{
		readonly PatternKey _model;
		internal OctFormatter(PatternKey model) => _model = model;

		protected override string Convert(int t, int u)
		{
			if (t >= (int)PatternNote.C) return _model.Oct.Info.Format(u);
			return new string(_model.Note.Info.Format(t)[0], 1);
		}
	}
}