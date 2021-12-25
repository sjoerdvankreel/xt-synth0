using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class UnitDSP
	{
		const int MaxHarmonics = 32;
		static readonly float AdditiveScale = 2.0f / (MathF.Log(MaxHarmonics) + 1.0f);

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

		public float Next(UnitModel unit, SynthMethod method, float rate)
		{
			if (unit.On.Value == 0) return 0.0f;
			float amp = unit.Amp.Value / 255.0f;
			float freq = Frequency(unit);
			var type = (UnitType)unit.Type.Value;
			float sample = Generate(method, type, freq, rate);
			_phase += freq / rate;
			if (_phase >= 1.0f) _phase = 0.0f;
			return sample * amp;
		}

		float Generate(SynthMethod method, UnitType type, float freq, float rate)
		=> type switch
		{
			UnitType.Sin => MathF.Sin(_phase * MathF.PI * 2.0f),
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
			UnitType.Saw => _phase * 2.0f - 1.0f,
			UnitType.Sqr => _phase > 0.5f ? 1.0f : -1.0f,
			UnitType.Tri => (_phase <= 0.5f ? _phase : 1.0f - _phase) * 4.0f - 1.0f,
			_ => throw new InvalidOperationException()
		};

		float GenerateAdditive(UnitType type, float freq, float rate)
		=> type switch
		{
			UnitType.Saw => GenerateAdditiveSaw(freq, rate),
			_ => 0.0f
		};

		float GenerateAdditiveSaw(float freq, float rate)
		{
			float result = 0.0f;
			float nyquist = rate / 2.0f;
			for (int h = 1; h <= MaxHarmonics; h++)
			{
				if (h * freq >= nyquist) return result * AdditiveScale;
				result += MathF.Sin(_phase * h * MathF.PI * 2.0f) / h;
			}
			return result * AdditiveScale;
		}

		float GeneratePolyBlep(UnitType type) => 0.0f;
	}
}