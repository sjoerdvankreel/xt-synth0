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

		public float Next(UnitModel unit, SynthMethod method, float rate)
		{
			if (unit.On.Value == 0) return 0.0f;
			float amp = unit.Amp.Value / 255.0f;
			float freq = Frequency(unit);
			float phase = _phase;
			_phase += freq / rate;
			if (_phase >= 1.0f) _phase = 0.0f;
			var type = (UnitType)unit.Type.Value;
			return Generate(method, type, phase) * amp;
		}

		float Generate(SynthMethod method, UnitType type, float phase) => method switch
		{
			SynthMethod.Nve => GenerateNaive(type, phase),
			SynthMethod.Add => GenerateAdditive(type, phase),
			SynthMethod.PBP => GeneratePolyBlep(type, phase),
			_ => throw new InvalidOperationException()
		};

		float GenerateNaive(UnitType type, float phase) => type switch
		{
			UnitType.Saw => phase * 2.0f - 1.0f,
			UnitType.Sqr => phase > 0.5f ? 1.0f : -1.0f,
			UnitType.Sin => MathF.Sin(phase * MathF.PI * 2.0f),
			UnitType.Tri => (phase <= 0.5f ? phase : 0.5f - phase) * 4.0f - 1.0f,
			_ => throw new InvalidOperationException()
		};

		float GenerateAdditive(UnitType type, float phase) => 0.0f;
		float GeneratePolyBlep(UnitType type, float phase) => 0.0f;
	}
}