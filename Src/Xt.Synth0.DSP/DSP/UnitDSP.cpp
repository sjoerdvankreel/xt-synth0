#include "DSP.hpp"
#include "UnitDSP.hpp"

#include <cmath>
#include <cassert>
#include <immintrin.h>

namespace Xts {

const int NoteCount = 12;

static inline int
NoteNum(int oct, UnitNote note)
{ 
  int n = static_cast<int>(note);
  return oct * NoteCount + n; 
}

static inline float
GetFreq(int noteNum, int cent)
{
  float midi = noteNum + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

float
UnitDSP::PwPhase(int pw) const
{
	float phase = static_cast<float>(_phase);
	float result = phase + Mix01Exclusive(pw);
	return result - (int)result;
}

float
UnitDSP::Freq(UnitModel const& model) const
{ 
  int cent = static_cast<int>(Mix0100Inclusive(model.dtn));
  int note = NoteNum(model.oct + 1, model.note) + _noteOffset;
  return GetFreq(note, cent);
}

void
UnitDSP::Init(UnitModel const& model, AudioInput const& input)
{
	_phase = 0.0;
	int note = NoteNum(input.oct, input.note);
  int baseNote = NoteNum(4, UnitNote::C);
	_noteOffset = note - baseNote;
}

void
UnitDSP::Plot(UnitModel const& model, PlotInput const& input, PlotOutput& output)
{
	output.clip = false;
	output.bipolar = true;
	output.freq = Freq(model);
	output.rate = output.freq * input.pixels;

	float samples = output.rate / output.freq;
	AudioInput in(output.rate, input.bpm, 4, UnitNote::C);
	Init(model, in);
	for (int i = 0; i <= static_cast<int>(samples); i++)
		output.samples->push_back(Next(model, in).Mono());
}

AudioOutput
UnitDSP::Next(UnitModel const& model, AudioInput const& input)
{
	if (model.type == UnitType::Off) return AudioOutput(0.0f, 0.0f);
	float freq = Freq(model);
	float amp = Level(model.amp);
	float pan = Mix01Inclusive(model.pan);
	float sample = Generate(model, freq, input.rate);
	_phase += freq / input.rate;
	if (_phase >= 1.0) _phase = 0.0;
  return AudioOutput(sample * amp * pan, sample * amp * (1.0f - pan));
}

float 
UnitDSP::Generate(UnitModel const& model, float freq, float rate) const
{
	auto phase = static_cast<float>(_phase);
	switch(model.type)
  {
	case UnitType::Sin: return std::sinf(phase * 2.0f * PI);
	case UnitType::Add: return GenerateAdd(model, freq, rate);
	case UnitType::Naive: return GenerateNaive(model.naiveType, model.pw, phase);
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
UnitDSP::GenerateAdd(UnitModel const& model, float freq, float rate) const
{
  AddType type = model.addType;
  int maxParts = Exp(model.addMaxParts);
	auto phase = static_cast<float>(_phase);
	bool sinCos = type == AddType::SinAddCos || type == AddType::SinSubCos;
	bool addSub = type == AddType::SinSubSin || type == AddType::SinSubCos;
	switch(type)
  {
	case AddType::Pulse: break;
	case AddType::Tri: return GenerateAdd(freq, rate, phase, maxParts, 2, 2.0f, true, false);
	case AddType::Saw: return GenerateAdd(freq, rate, phase, maxParts, 1, 1.0f, false, false);
	case AddType::Sqr: return GenerateAdd(freq, rate, phase, maxParts, 2, 1.0f, false, false);
	case AddType::Impulse: return GenerateAdd(freq, rate, phase, maxParts, 1, 0.0f, false, false);
  default: return GenerateAdd(freq, rate, phase, model.addParts, model.addStep, addSub, sinCos, Mix02Inclusive(model.addRoll));
	}
  float saw1 = GenerateAdd(freq, rate, phase, maxParts, 1, 1.0f, false, false);
	float saw2 = GenerateAdd(freq, rate, PwPhase(model.pw), maxParts, 1, 1.0f, false, false);
	return (saw1 - saw2) / 2.0f;
}

float 
UnitDSP::GenerateAdd(float freq, float rate, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const
{
	__m256 cosines;
	float limit = 0.0;
	float result = 0.0;
	const int selector = 0b01010101;

  __m256 ones = _mm256_set1_ps(1.0f);
	__m256 zeros = _mm256_set1_ps(0.0f);
	__m256 signs = _mm256_set1_ps(1.0f);
	__m256 freqs = _mm256_set1_ps(freq);
	__m256 limits = _mm256_set1_ps(0.0f);
	__m256 results = _mm256_set1_ps(0.0f);
	__m256 phases = _mm256_set1_ps(phase);
  __m256 twopis = _mm256_set1_ps(2.0f * PI);
	__m256 logRolls = _mm256_set1_ps(logRoll);
	__m256 nyquists = _mm256_set1_ps(rate / 2.0f);
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