using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class SynthDSP
	{
		int _previousRow = -1;
		double _rowFactor = 0.0f;
		readonly PatternDSP _pattern = new PatternDSP();
		readonly UnitDSP[] _units = new UnitDSP[SynthModel.UnitCount];

		public SynthDSP()
		{
			for (int u = 0; u < _units.Length; u++)
				_units[u] = new UnitDSP();
		}

		public void Reset(AudioModel audio)
		{
			_previousRow = -1;
			_rowFactor = 0.0f;
			audio.CurrentRow = 0;
			foreach (var unit in _units)
				unit.Reset();
		}

		bool RowUpdated(AudioModel audio)
		{
			bool result = _previousRow != audio.CurrentRow;
			_previousRow = audio.CurrentRow;
			return result;
		}

		public float Next(SynthModel synth, AudioModel audio, float rate)
		{
			float result = 0.0f;
			float amp = synth.Amp.Lvl.Value / 255.0f;
			if (UpdateRow(synth, audio, rate))
				_pattern.Automate(synth, audio);
			for (int u = 0; u < _units.Length; u++)
				result += _units[u].Next(synth.Global, synth.Units[u], rate) * amp;
			return result;
		}

		bool UpdateRow(SynthModel synth, AudioModel audio, float rate)
		{
			int bpm = synth.Global.Bpm.Value;
			int patterns = synth.Track.Pats.Value;
			int rowsPerPattern = PatternModel.PatternRows;
			int totalRows = patterns * rowsPerPattern;
			_rowFactor += bpm * PatternModel.BeatRows / (60.0 * rate);
			if (_rowFactor < 1.0) return RowUpdated(audio);
			_rowFactor = 0.0f;
			if (audio.CurrentRow < totalRows - 1) audio.CurrentRow++;
			else audio.CurrentRow = 0;
			return RowUpdated(audio);
		}
	}
}