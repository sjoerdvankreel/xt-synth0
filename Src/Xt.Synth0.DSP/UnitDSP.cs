using System;
using Xt.Synth0.Model;

namespace Xt.Synth0.DSP
{
	public class UnitDSP
	{
		static readonly float[] SineTable = MakeSineTable();
		static readonly float[,,] FrequencyTable = MakeFrequencyTable();

		static float Frequency(int oct, int note, int cent)
		{
			float midi = (oct + 1) * 12 + note + cent / 100.0f;
			return 440.0f * MathF.Pow(2.0f, (midi - 69.0f) / 12.0f);
		}

		static float[,,] MakeFrequencyTable()
		{
			const int notes = 12;
			const int cents = 100;
			const int octaves = UnitModel.MaxOctave - UnitModel.MinOctave + 1;
			float[,,] result = new float[octaves, notes, cents];
			for (int oct = 0; oct < octaves; oct++)
				for (int note = 0; note < notes; note++)
					for (int cent = -50; cent < 50; cent++)
						result[oct, note, cent + 50] = Frequency(oct, note, cent);
			return result;
		}

		static float Sin(double phase)
		{
			return SineTable[(int)((phase - (int)phase) * SineTable.Length)];
			double p = phase - (int)phase;
			double x = p * SineTable.Length;
			int x0 = ((int)x) % SineTable.Length;
			int x1 = (x0 + 1) % SineTable.Length;
			float weight = (float)(x - x0);
			return SineTable[x0] * (1.0f - weight) + SineTable[x1] * weight;
		}

		static float[] MakeSineTable()
		{
			var result = new float[65535];
			for (int i = 0; i < result.Length; i++)
				result[i] = MathF.Sin(i / (float)result.Length * 2.0f * MathF.PI);
			return result;
		}

		double _phase = 0.0;
		internal void Reset() => _phase = 0.0;

		public float Frequency(UnitModel unit)
		{
			int oct = unit.Oct.Value;
			int note = unit.Note.Value;
			int cent = unit.Cent.Value;
			return FrequencyTable[oct, note, cent + 50];
		}

		public float Next(GlobalModel global, UnitModel unit, float rate)
		{
			if (unit.On.Value == 0) return 0.0f;
			float amp = unit.Amp.Value / 255.0f;
			float freq = Frequency(unit);
			var type = (UnitType)unit.Type.Value;
			float sample = Generate(global, type, freq, rate);
			_phase += freq / rate;
			if (_phase >= 1.0) _phase = 0.0;
			return sample * amp;
		}

		float Generate(GlobalModel global, UnitType type, float freq, float rate)
		=> type switch
		{
			UnitType.Sin => Sin(_phase),
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
			UnitType.Saw => (float)(_phase * 2.0 - 1.0),
			UnitType.Sqr => (float)(_phase > 0.5 ? 1.0 : -1.0),
			UnitType.Tri => (float)((_phase <= 0.5 ? _phase : 1.0 - _phase) * 4.0 - 1.0),
			_ => throw new InvalidOperationException()
		};

		float GenerateAdditive(UnitType type, float freq, float rate, int logHarmonics)
		=> type switch
		{
			UnitType.Saw => GenerateAdditive(freq, rate, logHarmonics, 1, 1, 0),
			UnitType.Sqr => GenerateAdditive(freq, rate, logHarmonics, 2, 1, 0),
			UnitType.Tri => GenerateAdditive(freq, rate, logHarmonics, 2, -1, 1),
			_ => throw new InvalidOperationException()
		};

		float GenerateAdditive(float freq, float rate, int logHarmonics, int step, int multiplier, int logRolloff)
		{
			int sign = 1;
			int harmonics = 1;
			float limit = 0.0f;
			float result = 0.0f;
			float nyquist = rate / 2.0f;
			for (int h = 0; h < logHarmonics; h++)
				harmonics *= 2;
			for (int h = 1; h <= harmonics * step; h += step)
			{
				if (h * freq >= nyquist) break;
				int rolloff = h;
				for (int r = 0; r < logRolloff; r++)
					rolloff *= h;
				float amp = 1.0f / rolloff;
				limit += amp;
				//result += sign * MathF.Sin((float)_phase * h * 2.0f * MathF.PI) * amp;
				var p = _phase * h;
				result += sign * amp * SineTable[(int)((p - (int)p) * SineTable.Length)];
				sign *= multiplier;
			}
			return result / limit;
		}

		float GeneratePolyBlep(UnitType type) => 0.0f;
	}
}