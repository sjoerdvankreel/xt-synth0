using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class SequencerDSP
	{
		int _previousRow = -1;
		double _rowFactor = 0.0f;
		readonly SynthDSP _synthDSP = new();
		readonly PatternDSP _patternDSP = new();

		public void Reset(AudioModel audio)
		{
			_previousRow = -1;
			_rowFactor = 0.0f;
			audio.CurrentRow = 0;
			_synthDSP.Reset();
		}

		bool RowUpdated(AudioModel audio)
		{
			bool result = _previousRow != audio.CurrentRow;
			_previousRow = audio.CurrentRow;
			return result;
		}

		public float Next(AudioModel audio, 
			SequencerModel seq, SynthModel synth, float rate)
		{
			if (UpdateRow(audio, seq, synth, rate))
				_patternDSP.Automate(audio, seq, synth);
			return _synthDSP.Next(synth, rate);
		}

		bool UpdateRow(AudioModel audio,
			SequencerModel seq, SynthModel synth, float rate)
		{
			int bpm = synth.Global.Bpm.Value;
			int patterns = seq.Edit.Pats.Value;
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