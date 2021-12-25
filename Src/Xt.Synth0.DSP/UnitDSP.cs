using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class UnitDSP
	{
		const int MaxHarmonics = 32;
		static readonly float AdditiveScale = 2.0f / (MathF.Log(MaxHarmonics) + 1.0f);

		readonly UnitModel _previous = new UnitModel();
		readonly float[] _phases = new float[MaxHarmonics];

		internal void Reset() => Array.Clear(_phases);

		void TryReset(UnitModel unit)
		{
			if (unit.On.Value != _previous.On.Value ||
				unit.Oct.Value != _previous.Oct.Value ||
				unit.Note.Value != _previous.Note.Value ||
				unit.Cent.Value != _previous.Cent.Value ||
				unit.Type.Value != _previous.Type.Value)
				Reset();
			unit.CopyTo(_previous);
		}

		public float Frequency(UnitModel unit)
		{
			int oct = unit.Oct.Value;
			int note = unit.Note.Value;
			int cent = unit.Cent.Value;
			float midi = (oct + 1) * 12 + note + cent / 100.0f;
			return 440.0f * MathF.Pow(2.0f, (midi - 69.0f) / 12.0f);
		}

		public float Next(UnitModel unit, SynthMethod method, float rate)
		{
			TryReset(unit);
			if (unit.On.Value == 0) return 0.0f;
			float amp = unit.Amp.Value / 255.0f;
			float freq = Frequency(unit);
			var type = (UnitType)unit.Type.Value;
			float sample = Generate(method, type, freq, rate);
			UpdatePhases(freq, rate);
			return sample * amp;
		}

		void UpdatePhases(float freq, float rate)
		{
			for (int h = 0; h < MaxHarmonics; h++)
			{
				_phases[h] += freq * (h + 1) / rate;
				if (_phases[h] >= 1.0f) _phases[h] = 0.0f;
			}
		}

		float Generate(SynthMethod method, UnitType type, float freq, float rate)
		=> type switch
		{
			UnitType.Sin => MathF.Sin(_phases[0] * MathF.PI * 2.0f),
			_ => GenerateMethod(method, type, freq, rate)
		};

		float GenerateMethod(SynthMethod method, UnitType type, float freq, float rate)
		=> method switch
		{
			SynthMethod.Naive => GenerateNaive(type),
			SynthMethod.Additive => GenerateAdditive(type, freq, rate),
			SynthMethod.PolyBlep => GeneratePolyBlep(type),
			_ => throw new InvalidOperationException()
		};

		float GenerateNaive(UnitType type)
		=> type switch
		{
			UnitType.Saw => _phases[0] * 2.0f - 1.0f,
			UnitType.Sqr => _phases[0] > 0.5f ? 1.0f : -1.0f,
			UnitType.Tri => (_phases[0] <= 0.5f ? _phases[0] : 1.0f - _phases[0]) * 4.0f - 1.0f,
			_ => throw new InvalidOperationException()
		};

		float GenerateAdditive(UnitType type, float freq, float rate)
		=> type switch
		{
			UnitType.Saw => -GenerateAdditiveSaw(freq, rate),
			_ => 0.0f
		};

		float GenerateAdditiveSaw(float freq, float rate)
		{
			float result = 0.0f;
			float nyquist = rate / 2.0f;
			for (int h = 1; h <= MaxHarmonics; h++)
			{
				if (h * freq >= nyquist) return result * AdditiveScale;
				result += MathF.Sin(_phases[h - 1] * MathF.PI * 2.0f) / h;
			}
			return result * AdditiveScale;
		}

		float GeneratePolyBlep(UnitType type) => 0.0f;
	}
}