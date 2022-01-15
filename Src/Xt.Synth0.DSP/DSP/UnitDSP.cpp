#include "UnitDSP.hpp"
#include "DSP.hpp"
#include <cassert>
#define _USE_MATH_DEFINES 1
#include <cmath>
#include <immintrin.h>

namespace Xts {

const int OctCount = 10;
const int NoteCount = 12;
const int CentCount = 101;
static float FrequencyTable[OctCount][NoteCount][CentCount];

static inline int
NoteNum(int oct, int note)
{ return (oct + 1) * NoteCount + note; }

static inline float
GetFrequency(int oct, int note, int cent)
{
  float midi = NoteNum(oct, note) + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

void
UnitDSP::Init()
{
	for (int oct = 0; oct < OctCount; oct++)
		for (int note = 0; note < NoteCount; note++)
			for (int cent = 0; cent < CentCount; cent++)
				FrequencyTable[oct][note][cent] = GetFrequency(oct, note, cent - 50);
}

void
UnitDSP::Reset()
{ _phase = 0.0; }

float 
UnitDSP::PwPhase(float phase, int pw) const
{
	float result = phase + Mix01Exclusive(pw);
	return result - (int)result;
}

float
UnitDSP::Frequency(UnitModel const& unit) const
{ 
  int cent = -50 + static_cast<int>(Mix0100Inclusive(unit.dtn));
  return FrequencyTable[unit.oct][unit.note][cent + 50]; 
}

void
UnitDSP::Next(UnitModel const& unit, float rate, int octave, PatternNote note, bool tick, bool plot, float* l, float* r, bool* cycled)
{
	*l = 0.0f;
  *r = 0.0f;
	*cycled = false;
	auto off = static_cast<int>(UnitType::Off);
  if(!plot && unit.type == off) return;
  if(tick) Reset();
	float amp = Level(unit.amp);
	float freq = Frequency(unit);
  float pan = Mix01Inclusive(unit.pan);
	float phase = static_cast<float>(_phase);
	float sample = Generate(unit, freq, rate, phase);
	_phase += freq / rate;
	if (_phase >= 1.0)
  { 
    Reset();
    *cycled = true;
  }
  *l = sample * amp * (1.0f - pan);
  *r = sample * amp * pan;
}

float 
UnitDSP::Generate(UnitModel const& unit, float freq, float rate, float phase) const
{
	auto pi = static_cast<float>(M_PI);
	auto type = static_cast<UnitType>(unit.type);
  auto naiveType = static_cast<NaiveType>(unit.naiveType);
	switch(type)
  {
  case UnitType::Off: return 0.0f;
	case UnitType::Sin: return std::sinf(phase * 2.0f * pi);
	case UnitType::Add: return GenerateAdd(unit, freq, rate, phase);
	case UnitType::Naive: return GenerateNaive(naiveType, unit.pw, phase);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateNaive(NaiveType type, int pw, float phase) const
{
  switch(type)
  {
    case NaiveType::Saw: return phase * 2.0f - 1.0f;
		case NaiveType::Tri: return (phase < 0.5f ? phase : 1.0f - phase) * 4.0f - 1.0f;
		case NaiveType::Pulse: return (GenerateNaive(NaiveType::Saw, pw, phase)
			- GenerateNaive(NaiveType::Saw, pw, PwPhase(phase, pw))) / 2.0f;
		default: assert(false); return 0.0f;
	}
}

float
UnitDSP::GenerateAdd(UnitModel const& unit, float freq, float rate, float phase) const
{
  int step;
	int parts;
	bool sinCos;
	bool addSub;
	float logRoll;
  auto type = static_cast<AddType>(unit.addType);
  switch(type)
  {
  case AddType::Saw: step = 1; sinCos = false, addSub = false, logRoll = 1.0f; parts = 1 << unit.addMaxParts; break;
	case AddType::Tri: step = 2; sinCos = false, addSub = true, logRoll = 2.0f;	parts = 1 << unit.addMaxParts;	break;
	case AddType::Sqr: step = 2;	sinCos = false, addSub = false, logRoll = 1.0f; parts = 1 << unit.addMaxParts; break;
	case AddType::Pulse: step = 1; sinCos = false, addSub = false, logRoll = 1.0f; parts = 1 << unit.addMaxParts; break;
	case AddType::Impulse:	step = 1; sinCos = false, addSub = false, logRoll = 0.0f; parts = 1 << unit.addMaxParts; break;
	case AddType::SinAddSin:
	case AddType::SinAddCos:
	case AddType::SinSubSin:
	case AddType::SinSubCos:
    step = unit.addStep;
		parts = unit.addParts;
		logRoll = Mix02Inclusive(unit.addRoll);
		sinCos = type == AddType::SinAddCos || type == AddType::SinSubCos;
		addSub = type == AddType::SinSubSin || type == AddType::SinSubCos;
    break;
  default: assert(false); break;
	}
  float result = GenerateAdd(freq, rate, phase, addSub, sinCos, parts, step, logRoll);
  if(type != AddType::Pulse) return result;
	float phase2 = PwPhase(phase, unit.pw);
  return (result - GenerateAdd(freq, rate, phase2, addSub, sinCos, parts, step, logRoll)) / 2.0f;
}

float 
UnitDSP::GenerateAdd(float freq, float rate, float phase, bool addSub, bool sinCos, int parts, int step, float logRoll) const
{
	__m256 cosines;
	float limit = 0.0;
	float result = 0.0;
	const int selector = 0b01010101;

	float pi = static_cast<float>(M_PI);
  __m256 ones = _mm256_set1_ps(1.0f);
	__m256 zeros = _mm256_set1_ps(0.0f);
	__m256 signs = _mm256_set1_ps(1.0f);
	__m256 freqs = _mm256_set1_ps(freq);
	__m256 limits = _mm256_set1_ps(0.0f);
	__m256 results = _mm256_set1_ps(0.0f);
	__m256 phases = _mm256_set1_ps(phase);
  __m256 twopis = _mm256_set1_ps(2.0f * pi);
	__m256 nyquists = _mm256_set1_ps(rate / 2.0f);
  __m256 logRolls = _mm256_set1_ps(logRoll);
	__m256 maxPs = _mm256_set1_ps(parts * static_cast<float>(step));
	if(addSub) signs = _mm256_set_ps(1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f);
	for (int p = 1; p <= parts * step; p += step * 8)
	{
    if(p * freq >= rate / 2.0f) break;
    __m256 allPs = _mm256_set_ps(
      p + 0.0f * step, p + 1.0f * step, p + 2.0f * step, p + 3.0f * step,
			p + 4.0f * step, p + 5.0f * step,	p + 6.0f * step, p + 7.0f * step);
		__m256 belowMax = _mm256_cmp_ps(allPs, maxPs, _CMP_LE_OQ);
		__m256 belowNyquists = _mm256_cmp_ps(_mm256_mul_ps(allPs, freqs), nyquists, _CMP_LT_OQ);
    __m256 wantedPs = _mm256_blendv_ps(zeros, _mm256_blendv_ps(zeros, ones, belowMax), belowNyquists);
  	__m256 rolls = _mm256_pow_ps(allPs, logRolls);
    __m256 amps = _mm256_div_ps(ones, rolls);
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