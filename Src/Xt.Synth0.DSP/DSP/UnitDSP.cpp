#include "UnitDSP.hpp"
#include "DSP.hpp"
#include <cassert>
#include <cstring>
#include <immintrin.h>
#define _USE_MATH_DEFINES 1
#include <cmath>

namespace Xts {

const int OctCount = 10;
const int NoteCount = 12;
const int CentCount = 101;
static float FrequencyTable[OctCount * NoteCount][CentCount];

static inline int
NoteNum(int oct, int note)
{ return oct * NoteCount + note; }

static inline float
GetFrequency(int oct, int note, int cent)
{
  float midi = NoteNum(oct, note) + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

float
UnitDSP::PwPhase(int pw) const
{
	float phase = static_cast<float>(_phase);
	float result = phase + Mix01Exclusive(pw);
	return result - (int)result;
}

void
UnitDSP::Init()
{
	for (int oct = 0; oct < OctCount; oct++)
		for (int note = 0; note < NoteCount; note++)
			for (int cent = 0; cent < CentCount; cent++)
				FrequencyTable[NoteNum(oct, note)][cent] 
          = GetFrequency(oct, note, cent - 50);
}

void
UnitDSP::Init(int oct, UnitNote note)
{ 
	_phase = 0.0;
	int c = static_cast<int>(UnitNote::C);
  int index = NoteNum(oct + 1, static_cast<int>(note));
  _noteOffset = index - NoteNum(4, c);
}

float
UnitDSP::Freq(UnitModel const& unit) const
{ 
  int cent = static_cast<int>(Mix0100Inclusive(unit.dtn));
  int note = NoteNum(unit.oct + 1, unit.note) + _noteOffset;
  return FrequencyTable[note][cent];
}

void
UnitDSP::Next(UnitModel const& unit, float rate, UnitOutput& output)
{
	memset(&output, 0, sizeof(output));
	auto off = static_cast<int>(UnitType::Off);
	if (unit.type == off) return;

	float freq = Freq(unit);
	float amp = Level(unit.amp);
	float pan = Mix01Inclusive(unit.pan);
	float sample = Generate(unit, freq, rate);

	_phase += freq / rate;
	output.cycled = _phase >= 1.0;
	if (_phase >= 1.0) _phase = 0.0;
	output.r = sample * amp * pan;
	output.l = sample * amp * (1.0f - pan);
}

float 
UnitDSP::Generate(UnitModel const& unit, float freq, float rate) const
{
	auto pi = static_cast<float>(M_PI);
	auto phase = static_cast<float>(_phase);
	auto type = static_cast<UnitType>(unit.type);
  auto naive = static_cast<NaiveType>(unit.naiveType);
	switch(type)
  {
  case UnitType::Off: return 0.0f;
	case UnitType::Sin: return std::sinf(phase * 2.0f * pi);
	case UnitType::Add: return GenerateAdd(unit, freq, rate);
	case UnitType::Naive: return GenerateNaive(naive, unit.pw, phase);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateNaive(NaiveType type, int pw, float phase) const
{
  switch(type)
  {
    case NaiveType::Pulse: break;
    case NaiveType::Saw: return phase * 2.0f - 1.0f;
		case NaiveType::Tri: return (phase < 0.5f ? phase : 1.0f - phase) * 4.0f - 1.0f;
		default: assert(false); return 0.0f;
	}
	float saw1 = GenerateNaive(NaiveType::Saw, pw, phase);
	float saw2 = GenerateNaive(NaiveType::Saw, pw, PwPhase(pw));
	return (saw1 - saw2) / 2.0f;
}

float
UnitDSP::GenerateAdd(UnitModel const& unit, float freq, float rate) const
{
  AddParams params;
  int maxParts = Exp(unit.addMaxParts);
	auto phase = static_cast<float>(_phase);
	auto type = static_cast<AddType>(unit.addType);
	bool sinCos = type == AddType::SinAddCos || type == AddType::SinSubCos;
	bool addSub = type == AddType::SinSubSin || type == AddType::SinSubCos;
	switch(type)
  {
	case AddType::Pulse:
  case AddType::Saw: params = AddParams(maxParts, 1, false, false, 1.0f); break;
	case AddType::Tri: params = AddParams(maxParts, 2, true, false, 2.0f); break;
	case AddType::Sqr: params = AddParams(maxParts, 2, false, false, 1.0f); break;
	case AddType::Impulse: params = AddParams(maxParts, 1, false, false, 0.0f); break;
  default: params = AddParams(unit.addParts, unit.addStep, addSub, sinCos, Mix02Inclusive(unit.addRoll));
	}
  float result = GenerateAdd(freq, rate, phase, params);
  if(type != AddType::Pulse) return result;
  float saw2 = GenerateAdd(freq, rate, PwPhase(unit.pw), params);
	return (result - saw2) / 2.0f;
}

float 
UnitDSP::GenerateAdd(float freq, float rate, float phase, AddParams const& params) const
{
	__m256 cosines;
	float limit = 0.0;
	float result = 0.0;
	int step = params.step;
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
  __m256 logRolls = _mm256_set1_ps(params.logRoll);
	__m256 maxPs = _mm256_set1_ps(params.parts * static_cast<float>(step));
	if(params.addSub) signs = _mm256_set_ps(1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f);

	for (int p = 1; p <= params.parts * step; p += step * 8)
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
    __m256 waves = !params.sinCos? sines: _mm256_blend_ps(sines, cosines, selector);
    __m256 partialResults = _mm256_mul_ps(_mm256_mul_ps(waves, amps), signs);
		limits = _mm256_add_ps(limits, _mm256_mul_ps(amps, wantedPs));
		results = _mm256_add_ps(results, _mm256_mul_ps(partialResults, wantedPs));
	}
  for(int i = 0; i < params.parts && i < 8; i++)
  {
		limit += limits.m256_f32[7 - i];
		result += results.m256_f32[7 - i];
  }
	return result / limit;
}

} // namespace Xts