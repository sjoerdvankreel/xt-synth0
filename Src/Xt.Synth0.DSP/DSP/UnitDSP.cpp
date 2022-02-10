#include "DSP.hpp"
#include "UnitDSP.hpp"

#include <cmath>
#include <cassert>
#include <immintrin.h>

namespace Xts {

static inline float
Modulate(float val, float mod, float amt)
{ return val + (0.5f - std::fabs(val - 0.5f)) * amt * (mod * 2.0f - 1.0f); }

// http://www.martin-finke.de/blog/articles/audio-plugins-018-polyblep-oscillator/
static float
GenerateBlepSaw(float phase, float inc)
{
  if (phase >= 1.0f) phase -= 1.0f;
  float saw = 2.0f * phase - 1.0f;
  if(phase < inc) 
  {
    float blep = phase / inc;
    return saw - ((2.0f - blep) * blep - 1.0f);
  }
  if(phase >= 1.0f - inc)
  {
    float blep = (phase - 1.0f) / inc;
    return saw - ((blep + 2.0f) * blep + 1.0f);
  }
  return saw;
}

float
UnitDSP::Freq(UnitModel const& model, KeyInput const& input)
{
  int base = 4 * 12 + static_cast<int>(UnitNote::C);
  int key = input.oct * 12 + static_cast<int>(input.note);
  int unit = (model.oct + 1) * 12 + static_cast<int>(model.note);
  int noteNum = unit + key - base;
  int cent = Mix0100Inclusive(model.dtn);
  return Xts::Freq(noteNum + cent / 100.0f);
}

float
UnitDSP::Mod(SourceDSP const& source, ModSource mod) const
{
  int env = static_cast<int>(ModSource::Env1);
  int lfo = static_cast<int>(ModSource::LFO1);
  switch(mod)
  {
  case ModSource::Off: return 0.0f;
  case ModSource::LFO1: case ModSource::LFO2:
  return source.Lfos()[static_cast<int>(mod) - lfo].Value();
  case ModSource::Env1: case ModSource::Env2: case ModSource::Env3:
  return source.Envs()[static_cast<int>(mod) - env].Value();
  default: assert(false); return 0.0f;
  }
}

void
UnitDSP::Next(SourceDSP const& source)
{
  _value = AudioOutput();
  if (!_model->on) return;
  float mod1 = Mod(source, _model->src1);
  float mod2 = Mod(source, _model->src2);
  float freq = ModFreq(mod1, mod2);
  float phase = ModPhase(mod1, mod2);
  float sample = Generate(phase, freq, mod1, mod2);
  float pan = Modulate(ModTarget::Pan, _pan, mod1, mod2);
  float amp = Modulate(ModTarget::Amp, _amp, mod1, mod2);
  _phase += freq / _input->source.rate;
  _phase -= floor(_phase);
  assert(-1.0 <= sample && sample <= 1.0);
  _value = AudioOutput(sample * amp * (1.0f - pan), sample * amp * pan);
}

float
UnitDSP::Modulate(ModTarget tgt, float val, float mod1, float mod2) const
{
  assert(0.0f <= val && val <= 1.0f);
  assert(0.0f <= mod1 && mod1 <= 1.0f);
  assert(0.0f <= mod2 && mod2 <= 1.0f);

  float result = val;
  bool fst = _model->src1 != ModSource::Off;
  bool snd = _model->src2 != ModSource::Off;
  if(fst && _model->tgt1 == tgt) result = Xts::Modulate(result, mod1, _amt1);
  if(snd && _model->tgt2 == tgt) result = Xts::Modulate(result, mod2, _amt2);
  assert(0.0f <= result && result <= 1.0f);
  return result;
}

// https://www.musicdsp.org/en/latest/Synthesis/111-phase-modulation-vs-frequency-modulation.html
float
UnitDSP::ModPhase(float mod1, float mod2) const
{
  float result = static_cast<float>(_phase);
  bool fst = _model->src1 != ModSource::Off;
  bool snd = _model->src2 != ModSource::Off;
  if (fst && _model->tgt1 == ModTarget::Phase) result += _amt1 * (mod1 * 2.0f - 1.0f);
  if (snd && _model->tgt2 == ModTarget::Phase) result += _amt2 * (mod2 * 2.0f - 1.0f);
  return result - floorf(result);
}

// https://www.musicdsp.org/en/latest/Synthesis/111-phase-modulation-vs-frequency-modulation.html
float
UnitDSP::ModFreq(float mod1, float mod2) const
{
  float result = _freq;
  float freqRange = 12.0f;
  float pitchRange = 0.02930223f;
  bool fst = _model->src1 != ModSource::Off;
  bool snd = _model->src2 != ModSource::Off;
  if (fst && _model->tgt1 == ModTarget::Pitch) result += result * _amt1 * (mod1 * 2.0f - 1.0f) * pitchRange;
  assert(result > 0.0f);
  return result;
}

float
UnitDSP::Generate(float phase, float freq, float mod1, float mod2)
{
  switch (_model->type)
  {
  case UnitType::Add: return GenerateAdd(phase, freq, mod1, mod2);
  case UnitType::Blep: return GenerateBlep(phase, freq, mod1, mod2);
  default: assert(false); return 0.0f;
  }
}

float
UnitDSP::GenerateBlep(float phase, float freq, float mod1, float mod2)
{
  float result = 0.0f;
  const double BlepLeaky = 1.0e-4;
  float inc = freq / _input->source.rate;
  if (_model->blepType == BlepType::Saw)
  {
    result = GenerateBlepSaw(phase + 0.5f, inc);
    assert(-1.0f <= result && result <= 1.0f);
    return result;
  }
  float modPw = Modulate(ModTarget::Pw, _pw, mod1, mod2);
  float pwPhase = phase + 0.5f - modPw * 0.5f;
  pwPhase -= (int)pwPhase;
  if(_model->blepType == BlepType::Pulse)
  {
    float saw = GenerateBlepSaw(phase, inc);
    float result = (saw - GenerateBlepSaw(pwPhase, inc)) * 0.5f;
    assert(-1.0f <= result && result <= 1.0f);
    return result;
  }
  if(_model->blepType != BlepType::Tri)
    return assert(false), 0.0f;
  float saw = GenerateBlepSaw(phase + 0.25f, inc);
  float pulse = (saw - GenerateBlepSaw(pwPhase + 0.25f, inc)) * 0.5f;
  _blepTri = (1.0 - BlepLeaky) * _blepTri + inc * pulse;
  result = static_cast<float>(_blepTri) * (1.0f + modPw) * 4.0f;
  assert(-1.0f <= result && result <= 1.0f);
  return result;
}

float 
UnitDSP::GenerateAdd(float phase, float freq, float mod1, float mod2) const
{
  bool any = false;
  float limit = 0.0;
  float result = 0.0;
  int step = _model->addStep;
  int parts = _model->addParts;
  bool addSub = _model->addSub;
  float logRoll = Modulate(ModTarget::Roll, _roll, mod1, mod2) * 2.0f;

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
    __m256 sines = _mm256_sin_ps(_mm256_mul_ps(psPhases, twopis));
    __m256 partialResults = _mm256_mul_ps(_mm256_mul_ps(sines, amps), signs);
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

void
UnitDSP::Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output)
{
  const float cycles = 3.0f;
  if (!model.on) return;
  KeyInput key(4, UnitNote::C);
  output.max = 1.0f;
  output.min = -1.0f;
  output.freq = Freq(model, key);
  output.rate = input.spec? input.rate: output.freq * input.pixels;

  SourceInput sourceInput(output.rate, input.bpm);
  AudioInput audio(sourceInput, key);
  UnitDSP dsp(&model, &audio);
  SourceDSP sourceDsp(&source, &sourceInput);
  float regular = (output.rate * cycles / output.freq) + 1.0f;
  float fsamples = input.spec ? input.rate : regular;
  int samples = static_cast<int>(std::ceilf(fsamples));
  for (int i = 0; i < samples; i++)
  {
	sourceDsp.Next();
    dsp.Next(sourceDsp);
	output.samples->push_back(dsp.Value().Mono());
  }

  output.vSplits->emplace_back(VSplit(0.0f, L"0"));
  output.vSplits->emplace_back(VSplit(1.0f, L"-1"));
  output.vSplits->emplace_back(VSplit(-1.0f, L"1"));
  output.hSplits->emplace_back(HSplit(samples, L""));
  for(int i = 0; i < 6; i++)
	output.hSplits->emplace_back(HSplit(samples * i / 6, std::to_wstring(i) + L"\u03C0"));
}

} // namespace Xts