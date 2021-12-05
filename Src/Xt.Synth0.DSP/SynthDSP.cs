using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class SynthDSP
	{
		float _phase;

		public void Next(SynthModel model, float rate, float[] buffer, int frames)
		{
			UnitModel unit = model.Unit1;
			float midi = (unit.Oct.Value + 1) * 12 + unit.Note.Value + unit.Cent.Value / 100.0f;
			float freq = 440.0f * MathF.Pow(2.0f, (midi - 69.0f) / 12.0f);
			for (int i = 0; i < frames; i++)
			{
				var sample = (float)Math.Sin(_phase * Math.PI * 2.0f);
				buffer[i * 2] = sample;
				buffer[i * 2 + 1] = sample;
				_phase += freq / rate;
				if (_phase >= 1.0f) _phase = -1.0f;
			}
		}
	}
}