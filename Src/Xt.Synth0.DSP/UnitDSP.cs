using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	class UnitDSP
	{
		float _phase = 0.0f;
		internal void Reset() => _phase = 0.0f;

		internal float Next(UnitModel model, float rate)
		{
			if (model.On.Value == 0) return 0.0f;

			int oct = model.Oct.Value;
			int note = model.Note.Value;
			int cent = model.Cent.Value;

			float amp = model.Amp.Value / 255.0f;
			float midi = (oct + 1) * 12 + note + cent / 100.0f;
			float freq = 440.0f * MathF.Pow(2.0f, (midi - 69.0f) / 12.0f);

			float phase = _phase;
			_phase += freq / rate;
			if (_phase >= 1.0f) _phase = -1.0f;
			return Generator((UnitType)model.Type.Value, phase) * amp;
		}

		float Generator(UnitType type, float phase) => type switch
		{
			UnitType.Saw => phase * 2.0f - 1.0f,
			UnitType.Tri => phase * 2.0f - 1.0f,
			UnitType.Sqr => phase > 0.5f ? 1.0f : -1.0f,
			UnitType.Sin => MathF.Sin(phase * MathF.PI * 2.0f),
			_ => throw new InvalidOperationException()
		};
	}
}