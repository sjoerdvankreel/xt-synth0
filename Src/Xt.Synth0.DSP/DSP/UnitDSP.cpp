#include "UnitDSP.hpp"
#include <cassert>
#define _USE_MATH_DEFINES 1
#include <cmath>
#include <immintrin.h>

namespace Xts {

static constexpr int SineTableSize = 4096;
static constexpr int OctaveCount = TrackConstants::MaxOctave - TrackConstants::MinOctave + 1;
static float SineTable[SineTableSize];
static float FrequencyTable[OctaveCount][12][100];

static inline float
GetFrequency(int oct, int note, int cent)
{
  float midi = (oct + 1) * 12 + note + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

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
    case UnitType::Sin: return SineTable[static_cast<size_t>(_phasef * SineTableSize)];
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
	case UnitType::Saw: return GenerateAdditive(freq, rate, logHarmonics, 1, false);
	case UnitType::Sqr: return GenerateAdditive(freq, rate, logHarmonics, 2, false);
	case UnitType::Tri: return GenerateAdditive(freq, rate, logHarmonics, 2, true);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateAdditive(float freq, float rate, int logHarmonics, int step, bool tri)
{
	int harmonics = 1;
	float limit = 0.0;
	float result = 0.0;
  __m256 ones = _mm256_set1_ps(1.0f);
	__m256 signs = _mm256_set1_ps(1.0f);
	__m256 freqs = _mm256_set1_ps(freq);
	__m256 limits = _mm256_set1_ps(0.0f);
	__m256 results = _mm256_set1_ps(0.0f);
	__m256 phases = _mm256_set1_ps(_phasef);
	__m256 nyquists = _mm256_set1_ps(rate / 2.0f);
  __m256 tableSizes = _mm256_set1_ps(SineTableSize);
  if(tri)
   signs = _mm256_set_ps(1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f);
	for (int h = 0; h < logHarmonics; h++)
		harmonics *= 2;
	for (int h = 1; h <= harmonics * step; h += step * 8)
	{
    __m256 hs = _mm256_set_ps(
      h + 0 * step, h + 1 * step, h + 2 * step, h + 3 * step, 
      h + 4 * step, h + 5 * step, h + 6 * step, h + 7 * step);
    __m256 cmps = _mm256_cmp_ps(_mm256_mul_ps(hs, freqs), nyquists, _CMP_LT_OQ);
		if(!_mm256_movemask_ps(cmps)) break;
    __m256 rolloffs = tri? _mm256_mul_ps(hs, hs): hs;
    __m256 amps = _mm256_div_ps(ones, rolloffs);
    __m256 hsPhases = _mm256_mul_ps(phases, hs);
    __m256i hsTableIndices = _mm256_cvtps_epi32(_mm256_mul_ps(hsPhases, tableSizes));
    __m256 sines = _mm256_set_ps(
      SineTable[hsTableIndices.m256i_i32[7] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[6] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[5] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[4] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[3] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[2] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[1] % SineTableSize],
			SineTable[hsTableIndices.m256i_i32[0] % SineTableSize]);
    __m256 hmnsResults = _mm256_mul_ps(_mm256_mul_ps(sines, amps), signs);
		results = _mm256_add_ps(results, hmnsResults);
    limits = _mm256_add_ps(limits, amps);
	}
  for(int i = 0; i < harmonics && i < 8; i++)
  {
		limit += limits.m256_f32[7 - i];
		result += results.m256_f32[7 - i];
  }
	return result / limit;
}

} // namespace Xts