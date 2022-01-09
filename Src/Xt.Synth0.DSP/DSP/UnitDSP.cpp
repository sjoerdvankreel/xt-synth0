#include "UnitDSP.hpp"
#include <cassert>
#define _USE_MATH_DEFINES 1
#include <cmath>
#include <immintrin.h>

namespace Xts {

static constexpr int OctCount 
= TrackConstants::MaxOct - TrackConstants::MinOct + 1;
static float FrequencyTable[OctCount][12][100];

static inline float
GetFrequency(int oct, int note, int cent)
{
  float midi = (oct + 1) * 12 + note + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

void
UnitDSP::Init()
{
	const int notes = 12;
	const int cents = 100;
	const int octs = TrackConstants::MaxOct - TrackConstants::MinOct + 1;
	for (int oct = 0; oct < octs; oct++)
		for (int note = 0; note < notes; note++)
			for (int cent = -50; cent < 50; cent++)
				FrequencyTable[oct][note][cent + 50] = GetFrequency(oct, note, cent);
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
UnitDSP::Next(UnitModel const& unit, float rate)
{
	if (unit.type == static_cast<int>(UnitType::Off)) return 0.0f;
	_phasef = (float)_phased;
	float freq = Frequency(unit);
	float sample = Generate(unit, freq, rate);
	_phased += freq / rate;
	if (_phased >= 1.0) _phased = 0.0;
	return sample * unit.amp / 255.0f;
}

float 
UnitDSP::Generate(UnitModel const& unit, float freq, float rate)
{
	auto pi = static_cast<float>(M_PI);
	auto type = static_cast<UnitType>(unit.type);
  auto wave = static_cast<UnitWave>(unit.wave);
  auto addType = static_cast<AdditiveType>(unit.addType);
  switch(type)
  {
	case UnitType::Naive: return GenerateNaive(wave);
	case UnitType::Sin: return std::sinf(_phasef * 2.0f * pi);
	case UnitType::BasicAdd: return GenerateBasicAdd(unit, freq, rate);
	case UnitType::Additive: return GenerateAdditive(
    freq, rate, addType, unit.addParts, unit.addStep, unit.addRolloff / 128.0f);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateNaive(UnitWave wave)
{
  switch(wave)
  {
    case UnitWave::Saw: return _phasef * 2.0f - 1.0f;
		case UnitWave::Pulse: return _phasef < 0.5f ? 1.0f : -1.0f;
		case UnitWave::Tri: return (_phasef <= 0.5f ? _phasef : 1.0f - _phasef) * 4.0f - 1.0f;
		default: assert(false); return 0.0f;
	}
}

float
UnitDSP::GenerateBasicAdd(UnitModel const& unit, float freq, float rate)
{
  int partials = 1 << unit.basicAddLogParts;
	switch (static_cast<UnitWave>(unit.wave))
	{
	case UnitWave::Tri: return GenerateAdditive(freq, rate, AdditiveType::SinMinSin, partials, 2, 2.0f);
	case UnitWave::Saw: return GenerateAdditive(freq, rate, AdditiveType::SinPlusSin, partials, 1, 1.0f);
	case UnitWave::Pulse: return GenerateAdditive(freq, rate, AdditiveType::SinPlusSin, partials, 2, 1.0f);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateAdditive(float freq, float rate, AdditiveType type, int parts, int step, float logRolloff)
{
	__m256 cosines;
	float limit = 0.0;
	float result = 0.0;
	const int selector = 0b01010101;
  bool plusMin = type == AdditiveType::SinMinSin || type == AdditiveType::SinMinCos;
  bool sinCos = type == AdditiveType::SinPlusCos || type == AdditiveType::SinMinCos;

	float pi = static_cast<float>(M_PI);
  __m256 ones = _mm256_set1_ps(1.0f);
	__m256 zeros = _mm256_set1_ps(0.0f);
	__m256 signs = _mm256_set1_ps(1.0f);
	__m256 freqs = _mm256_set1_ps(freq);
	__m256 limits = _mm256_set1_ps(0.0f);
	__m256 results = _mm256_set1_ps(0.0f);
	__m256 phases = _mm256_set1_ps(_phasef);
  __m256 twopis = _mm256_set1_ps(2.0f * pi);
	__m256 maxPs = _mm256_set1_ps(parts * step);
	__m256 nyquists = _mm256_set1_ps(rate / 2.0f);
  __m256 logRolloffs = _mm256_set1_ps(logRolloff);
  if(plusMin) signs = _mm256_set_ps(1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f);
	for (int p = 1; p <= parts * step; p += step * 8)
	{
    if(p * freq >= rate / 2.0f) break;
    __m256 allPs = _mm256_set_ps(
      p + 0.0f * step, p + 1.0f * step, p + 2.0f * step, p + 3.0f * step,
			p + 4.0f * step, p + 5.0f * step,	p + 6.0f * step, p + 7.0f * step);
		__m256 belowMax = _mm256_cmp_ps(allPs, maxPs, _CMP_LE_OQ);
		__m256 belowNyquists = _mm256_cmp_ps(_mm256_mul_ps(allPs, freqs), nyquists, _CMP_LT_OQ);
    __m256 wantedPs = _mm256_blendv_ps(zeros, _mm256_blendv_ps(zeros, ones, belowMax), belowNyquists);
  	__m256 rolloffs = _mm256_pow_ps(allPs, logRolloffs);
    __m256 amps = _mm256_div_ps(ones, rolloffs);
    __m256 psPhases = _mm256_mul_ps(phases, allPs);
    __m256 sines = _mm256_sincos_ps(&cosines, _mm256_mul_ps(psPhases, twopis));
    __m256 waves = !sinCos? sines: _mm256_blend_ps(sines, cosines, selector);
    __m256 partialResults = _mm256_mul_ps(_mm256_mul_ps(waves, amps), signs);
		limits = _mm256_add_ps(limits, _mm256_mul_ps(amps, wantedPs));
		results = _mm256_add_ps(results, _mm256_mul_ps(partialResults, wantedPs));
	}
  for(int i = 0; i < parts && i < 8; i++)
  {
		limit += limits.m256_f32[7 - i];
		result += results.m256_f32[7 - i];
  }
	return result / limit;
}

} // namespace Xts