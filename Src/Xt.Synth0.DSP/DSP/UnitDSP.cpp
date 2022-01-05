#include "UnitDSP.hpp"
#include <cassert>
#define _USE_MATH_DEFINES 1
#include <cmath>

namespace Xts {

static constexpr int SineTableSize = 4096;
static constexpr int OctaveCount = TrackConstants::MaxOctave - TrackConstants::MinOctave + 1;
static float SineTable[SineTableSize];
static float FrequencyTable[OctaveCount][12][100];

static float
GetFrequency(int oct, int note, int cent)
{
  float midi = (oct + 1) * 12 + note + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

inline static float
Sin(double phase)
{ return SineTable[static_cast<size_t>(phase * SineTableSize)]; }

static void
InitFrequencyTable()
{
	const int notes = 12;
	const int cents = 100;
	const int octaves = TrackConstants::MaxOctave - TrackConstants::MinOctave + 1;
	for (int oct = 0; oct < octaves; oct++)
		for (int note = 0; note < notes; note++)
			for (int cent = -50; cent < 50; cent++)
				FrequencyTable[oct][note][cent + 50] = GetFrequency(oct, note, cent);
}

static void
InitSineTable()
{
  for(int i = 0; i < SineTableSize; i++)
    SineTable[i] = static_cast<float>(sin(i / (double)SineTableSize * 2.0 * M_PI));
}

void
UnitDSP::Init()
{
	InitSineTable();
	InitFrequencyTable();
}

void
UnitDSP::Reset()
{
  _phased = 0.0;
  _phasef = 0.0f;
}

float
UnitDSP::Frequency(UnitModel const& unit) const
{ return FrequencyTable[unit.oct][unit.note][unit.cent + 50]; }

float
UnitDSP::Next(GlobalModel const& global, UnitModel const& unit, float rate)
{
	if (unit.on == 0) return 0.0f;
	_phasef = (float)_phased;
	float freq = Frequency(unit);
	float amp = unit.amp / 255.0f;
	auto type = static_cast<UnitType>(unit.type);
	float sample = Generate(global, type, freq, rate);
	_phased += freq / rate;
	if (_phased >= 1.0) _phased = 0.0;
	return sample * amp;
}

float 
UnitDSP::Generate(GlobalModel const& global, UnitType type, float freq, float rate)
{
	auto pi = static_cast<float>(M_PI);
	switch(type)
  {
    case UnitType::Sin: return Sin(_phased);
    default: return GenerateMethod(global, type, freq, rate);
  }
}

float
UnitDSP::GenerateMethod(GlobalModel const& global, UnitType type, float freq, float rate)
{
  auto method = static_cast<SynthMethod>(global.method);
  switch(method)
  {
    case SynthMethod::PBP: return 0.0f;
		case SynthMethod::Nve: return GenerateNaive(type);
		case SynthMethod::Add: return GenerateAdditive(type, freq, rate, global.hmns);
    default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateNaive(UnitType type)
{
  switch(type)
  {
    case UnitType::Saw: return _phasef * 2.0f - 1.0f;
		case UnitType::Sqr: return _phasef > 0.5f ? 1.0f : -1.0f;
		case UnitType::Tri: return (_phasef <= 0.5f ? _phasef : 1.0f - _phasef) * 4.0f - 1.0f;
		default: assert(false); return 0.0f;
	}
}

float
UnitDSP::GenerateAdditive(UnitType type, float freq, float rate, int logHarmonics)
{
	switch (type)
	{
	case UnitType::Saw: return GenerateAdditive(freq, rate, logHarmonics, 1, 1, false);
	case UnitType::Sqr: return GenerateAdditive(freq, rate, logHarmonics, 2, 1, false);
	case UnitType::Tri: return GenerateAdditive(freq, rate, logHarmonics, 2, -1, true);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateAdditive(float freq, float rate, int logHarmonics, int step, int multiplier, bool sqrRolloff)
{
	int sign = 1;
	int harmonics = 1;
	float limit = 0.0f;
	float result = 0.0f;
	float nyquist = rate / 2.0f;
	auto pi = static_cast<float>(M_PI);
	for (int h = 0; h < logHarmonics; h++)
		harmonics *= 2;
	for (int h = 1; h <= harmonics * step; h += step)
	{
		if (h * freq >= nyquist) break;
		int rolloff = sqrRolloff? h * h: h;
		float amp = 1.0f / rolloff;
    double phase = _phased * h;
    phase -=static_cast<int>(phase);
		result += sign * Sin(phase) * amp;
		sign *= multiplier;
		limit += amp;
	}
	return result / limit;
}

} // namespace Xts