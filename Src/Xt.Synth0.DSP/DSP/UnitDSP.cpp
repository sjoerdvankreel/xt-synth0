#include "DSP.hpp"
#include "UnitDSP.hpp"

#include <cmath>
#include <cassert>
#include <immintrin.h>

namespace Xts {

float
UnitDSP::PwPhase() const
{
	float phase = static_cast<float>(_phase);
	float result = phase + Mix01Exclusive(_model->pw);
	return result - (int)result;
}

float
UnitDSP::Freq(UnitModel const& model, KeyInput const& input)
{
  int base = 4 * 12 + static_cast<int>(UnitNote::C);
	int key = input.oct * 12 + static_cast<int>(input.note);
	int unit = (model.oct + 1) * 12 + static_cast<int>(model.note);
	int noteNum = unit + key - base;
	int cent = Mix0100Inclusive(model.dtn);
	float midi = noteNum + cent / 100.0f;
	return 440.0f * powf(2.0f, (midi - 69.0f) / 12.0f);
}

void
UnitDSP::Next(SourceDSP const& source)
{
  _value = AudioOutput();
	if (!_model->on) return;
	float freq = Freq(*_model, _input->key);
	float sample = Generate(freq);
	float amp = Level(_model->amp);
	float pan = Mix01Inclusive(_model->pan);
	_phase += freq / _input->source.rate;
	_phase -= floor(_phase);
	assert(-1.0f <= sample && sample <= 1.0f);
	_value = AudioOutput(sample * amp * (1.0f - pan), sample * amp * pan);
}

float
UnitDSP::Generate(float freq) const
{
	auto phase = static_cast<float>(_phase);
	switch (_model->type)
	{
	case UnitType::Sin: return BasicSin(phase);
	case UnitType::Add: return GenerateAdd(freq);
	case UnitType::Naive: return GenerateNaive(_model->naiveType, phase);
	default: assert(false); return 0.0f;
	}
}

float 
UnitDSP::GenerateNaive(NaiveType type, float phase) const
{
  switch(type)
  {
    case NaiveType::Pulse: break;
    case NaiveType::Saw: return BasicSaw(phase);
		case NaiveType::Tri: return BasicTri(phase);
		default: assert(false); return 0.0f;
	}
	float saw = GenerateNaive(NaiveType::Saw, phase);
	return (saw - GenerateNaive(NaiveType::Saw, PwPhase())) / 2.0f;
}

void
UnitDSP::Plot(UnitModel const& model, PlotInput const& input, PlotOutput& output)
{
	if (!model.on) return;
	KeyInput key(4, UnitNote::C);
	output.bipolar = true;
	output.freq = Freq(model, key);
	output.rate = output.freq * input.pixels;

	SourceDSP sourceDsp;
	SourceInput sourceInput(output.rate, input.bpm);
	AudioInput audio(sourceInput, key);
	UnitDSP dsp(&model, &audio);
	float samples = output.rate / output.freq;
	for (int i = 0; i < static_cast<int>(samples); i++)
  {
    dsp.Next(sourceDsp);
		output.samples->push_back(dsp.Value().Mono());
  }
}

float
UnitDSP::GenerateAdd(float freq) const
{
  int step = _model->addStep;
	int parts = _model->addParts;
	AddType type = _model->addType;
	int maxParts = Exp(_model->addMaxParts);
	auto phase = static_cast<float>(_phase);
	float logRoll = Mix02Inclusive(_model->addRoll);
	bool sinCos = type == AddType::SinAddCos || type == AddType::SinSubCos;
	bool addSub = type == AddType::SinSubSin || type == AddType::SinSubCos;
	switch(type)
  {
	case AddType::Pulse: break;
	case AddType::Tri: return GenerateAdd(freq, phase, maxParts, 2, 2.0f, true, false);
	case AddType::Saw: return GenerateAdd(freq, phase, maxParts, 1, 1.0f, false, false);
	case AddType::Sqr: return GenerateAdd(freq, phase, maxParts, 2, 1.0f, false, false);
	case AddType::Impulse: return GenerateAdd(freq, phase, maxParts, 1, 0.0f, false, false);
  default: return GenerateAdd(freq, phase, parts, step, logRoll, addSub, sinCos);
	}
  float saw = GenerateAdd(freq, phase, maxParts, 1, 1.0f, false, false);
	return (saw - GenerateAdd(freq, PwPhase(), maxParts, 1, 1.0f, false, false)) / 2.0f;
}

float 
UnitDSP::GenerateAdd(float freq, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const
{
	__m256 cosines;
  bool any = false;
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
	__m256 nyquists = _mm256_set1_ps(_input->source.rate / 2.0f);
	__m256 maxPs = _mm256_set1_ps(parts * static_cast<float>(step));
	if(addSub) signs = _mm256_set_ps(1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f);

	for (int p = 1; p <= parts * step; p += step * 8)
	{
    if(p * freq >= _input->source.rate / 2.0f) break;
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
		any = true;
	}
  for(int i = 0; i < parts && i < 8; i++)
  {
		limit += limits.m256_f32[7 - i];
		result += results.m256_f32[7 - i];
  }
  if(!any) return 0.0f;
	result /= limit;
	assert(-1.0f <= result && result <= 1.0f);
  return result;
}

} // namespace Xts