using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class SynthDSP
	{
		int _currentFrame = 0;
		double _rowFactor = 0.0f;

		//readonly float[] _phases = new float[3];
		//readonly UnitModel[] _units = new UnitModel[3];

		int UpdateRow(AudioModel audio, float rate, int rows, int bpm)
		{
			_rowFactor += bpm * PatternModel.BeatRows / (60.0 * rate);
			if (_rowFactor < 1.0) return audio.CurrentRow;
			_rowFactor = 0.0f;
			if (audio.CurrentRow == rows - 1) return 0;
			return audio.CurrentRow + 1;
		}


		public void Next(SynthModel synth, AudioModel audio, float rate, float[] buffer, int frames)
		{
			int patterns = synth.Track.Pats.Value;
			int rowsPerPattern = PatternModel.PatternRows;
			int totalRows = patterns * rowsPerPattern;
			int totalBeats = totalRows / PatternModel.BeatRows;
			for (int f = 0; f < frames; f++)
			{
				int bpm = synth.Global.Bpm.Value;
				_rowFactor += bpm * PatternModel.BeatRows / (60.0 * rate);
				if (_rowFactor >= 1.0f)
				{
					_rowFactor = 0.0f;
					if (audio.CurrentRow == totalRows - 1)
						audio.CurrentRow = 0;
					else
						audio.CurrentRow++;
				}
				_currentFrame++;
				double totalFrames = totalBeats * 60.0 * rate / bpm;
				if (_currentFrame >= (int)totalFrames)
					_currentFrame = 0;
			}

			/*
			_units[0] = model.Units[0];
			_units[1] = model.Units[1];
			_units[2] = model.Units[2];
			for (int i = 0; i < frames * 2; i++)
			{
				buffer[i] = 0;
			}
			float synAmp = model.Amp.Lvl.Value / 255f;
			for (int u = 0; u < 3; u++)
			{
				UnitModel unit = _units[u];
				if (unit.On.Value == 0) continue;
				float midi = (unit.Oct.Value + 1) * 12 + unit.Note.Value + unit.Cent.Value / 100.0f;
				float freq = 440.0f * MathF.Pow(2.0f, (midi - 69.0f) / 12.0f);
				float unitAmp = unit.Amp.Value / 255f;
				for (int f = 0; f < frames; f++)
				{
					float sample = 0;
					switch (unit.Type.Value)
					{
						case (int)UnitType.Sin:
							sample = (float)Math.Sin(_phases[u] * Math.PI * 2.0f);
							break;
						case (int)UnitType.Saw:
							sample = _phases[u] * 2 - 1;
							break;
						case (int)UnitType.Tri:
							sample = _phases[u] * 2 - 1;
							break;
						case (int)UnitType.Sqr:
							sample = _phases[u] > 0.5f ? 1 : -1;
							break;
					}

					buffer[f * 2] += sample * unitAmp * synAmp;
					buffer[f * 2 + 1] += sample * unitAmp * synAmp;
					_phases[u] += freq / rate;
					if (_phases[u] >= 1.0f) _phases[u] = -1.0f;
				}
			}
			*/
		}
	}
}