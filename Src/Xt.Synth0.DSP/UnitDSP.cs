using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class UnitDSP
	{
		float _phase = 0.0f;
		internal void Reset() => _phase = 0.0f;

		public float Frequency(UnitModel unit)
		{
			int oct = unit.Oct.Value;
			int note = unit.Note.Value;
			int cent = unit.Cent.Value;
			float midi = (oct + 1) * 12 + note + cent / 100.0f;
			return 440.0f * MathF.Pow(2.0f, (midi - 69.0f) / 12.0f);
		}

		public float Next(GlobalModel global, UnitModel unit, float rate)
		{
			if (unit.On.Value == 0) return 0.0f;
			float amp = unit.Amp.Value / 255.0f;
			float freq = Frequency(unit);
			var type = (UnitType)unit.Type.Value;
			float sample = Generate(global, type, freq, rate);
			_phase += freq / rate;
			if (_phase >= 1.0f) _phase = 0.0f;
			return sample * amp;
		}

		float Generate(GlobalModel global, UnitType type, float freq, float rate)
		=> type switch
		{
			UnitType.Sin => MathF.Sin(_phase * MathF.PI * 2.0f),
			_ => GenerateMethod(global, type, freq, rate)
		};

		float GenerateMethod(GlobalModel global, UnitType type, float freq, float rate)
		=> (SynthMethod)global.Method.Value switch
		{
			SynthMethod.Nve => GenerateNaive(type),
			SynthMethod.PBP => GeneratePolyBlep(type),
			SynthMethod.Add => GenerateAdditive(type, freq, rate, global.Hmns.Value),
			_ => throw new InvalidOperationException()
		};

		float GenerateNaive(UnitType type)
		=> type switch
		{
			UnitType.Saw => _phase * 2.0f - 1.0f,
			UnitType.Sqr => _phase > 0.5f ? 1.0f : -1.0f,
			UnitType.Tri => (_phase <= 0.5f ? _phase : 1.0f - _phase) * 4.0f - 1.0f,
			_ => throw new InvalidOperationException()
		};

		float GenerateAdditive(UnitType type, float freq, float rate, int logHarmonics)
		=> type switch
		{
			UnitType.Tri => GenerateAdditiveTri(freq, rate, logHarmonics),
			UnitType.Saw => GenerateAdditiveSaw(freq, rate, logHarmonics),
			UnitType.Sqr => GenerateAdditiveSqr(freq, rate, logHarmonics),
			_ => 0.0f
		};

		float GenerateAdditiveSaw(float freq, float rate, int logHarmonics)
		{
			int harmonics = 1;
			float limit = 0.0f;
			float result = 0.0f;
			float nyquist = rate / 2.0f;
			for (int h = 0; h < logHarmonics; h++)
				harmonics *= 2;
			for (int h = 1; h <= harmonics; h++)
			{
				if (h * freq >= nyquist) break;
				float amp = 1.0f / h;
				limit += amp;
				result += MathF.Sin(_phase * h * MathF.PI * 2.0f) * amp;
			}
			return result / limit;
		}

		float GenerateAdditiveSqr(float freq, float rate, int logHarmonics)
		{
			int harmonics = 1;
			float limit = 0.0f;
			float result = 0.0f;
			float nyquist = rate / 2.0f;
			for (int h = 0; h < logHarmonics; h++)
				harmonics *= 2;
			for (int h = 1; h <= harmonics * 2; h += 2)
			{
				if (h * freq >= nyquist) break;
				float amp = 1.0f / h;
				limit += amp;
				result += MathF.Sin(_phase * h * MathF.PI * 2.0f) * amp;
			}
			return result / limit;
		}

		float GenerateAdditiveTri(float freq, float rate, int logHarmonics)
		{
			int sign = 1;
			int harmonics = 1;
			float limit = 0.0f;
			float result = 0.0f;
			float nyquist = rate / 2.0f;
			for (int h = 0; h < logHarmonics; h++)
				harmonics *= 2;
			for (int h = 1; h <= harmonics * 2; h += 2)
			{
				if (h * freq >= nyquist) break;
				float amp = 1.0f / (h * h);
				limit += amp;
				result += sign * MathF.Sin(_phase * h * MathF.PI * 2.0f) * amp;
				sign *= -1;
			}
			return result / limit;
		}

		float GeneratePolyBlep(UnitType type) => 0.0f;
	}
}