#include "DSP.hpp"
#include "UnitDSP.hpp"
#include "PlotDSP.hpp"
#include <DSP/Param.hpp>

#include <cmath>
#include <cassert>
#include <immintrin.h>

namespace Xts {

static const float MaxPw = 0.975f;

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

UnitDSP::
UnitDSP(UnitModel const* model, int oct, UnitNote note, float rate):
_output(),
_model(model),
_phase(0.0), _blepTri(0.0),
_rate(rate),
_pan(Param::Mix(model->pan)),
_amt1(Param::Mix(model->amt1)),
_amt2(Param::Mix(model->amt2)),
_amp(Param::Level(model->amp)),
_roll(Param::Mix(model->addRoll)),
_pw(Param::Level(model->pw) * MaxPw),
_freq(Freq(*model, oct, note)) {}

float
UnitDSP::Freq(UnitModel const& model, int oct, UnitNote note)
{
  float cent = Param::Mix(model.dtn) * 0.5f;
  int base = 4 * 12 + static_cast<int>(UnitNote::C);
  int key = oct * 12 + static_cast<int>(note);
  int unit = (model.oct + 1) * 12 + static_cast<int>(model.note);
  return MidiNoteFrequency(unit + key - base + cent);
}

float 
UnitDSP::Mod(UnitModTarget tgt, float val, bool bip, ModInput const& mod) const
{
  if (_model->tgt1 == tgt) val = Modulate(val, bip, _amt1, mod.cv1);
  if (_model->tgt2 == tgt) val = Modulate(val, bip, _amt2, mod.cv2);
  return val;
}

// https://www.musicdsp.org/en/latest/Synthesis/160-phase-modulation-vs-frequency-modulation-ii.html
float
UnitDSP::ModPhase(ModInput const& mod) const
{
  float phase = static_cast<float>(_phase);
  float base1 = mod.cv1.bipolar ? 0.5f : _amt1 >= 0.0f ? 0.0f : 1.0f;
  float base2 = mod.cv2.bipolar ? 0.5f : _amt2 >= 0.0f ? 0.0f : 1.0f;
  if (_model->tgt1 == UnitModTarget::Phase) phase += Modulate(base1, false, _amt1, mod.cv1);
  if (_model->tgt2 == UnitModTarget::Phase) phase += Modulate(base2, false, _amt2, mod.cv2);
  float result = phase - std::floorf(phase);
  assert(0.0f <= result && result <= 1.0f);
  return result;
}

// https://www.musicdsp.org/en/latest/Synthesis/160-phase-modulation-vs-frequency-modulation-ii.html
float
UnitDSP::ModFreq(ModInput const& mod) const
{
  float result = _freq;
  float minFreq = 10.0f;
  float maxFreq = 10000.0f;
  float pitchRange = 0.02930223f;
  float freqRange = maxFreq - minFreq;
  float freqBase = (std::max(minFreq, std::min(_freq, maxFreq)) - minFreq) / freqRange;
  if (_model->tgt1 == UnitModTarget::Pitch) result *= 1.0f + Modulate(0.0f, true, _amt1, mod.cv1) * pitchRange;
  if (_model->tgt1 == UnitModTarget::Freq) result = minFreq + Modulate(freqBase, false, _amt1, mod.cv1) * freqRange;
  if (_model->tgt2 == UnitModTarget::Pitch) result *= 1.0f + Modulate(0.0f, true, _amt2, mod.cv2) * pitchRange;
  if (_model->tgt2 == UnitModTarget::Freq) result = minFreq + Modulate(freqBase, false, _amt2, mod.cv2) * freqRange;
  assert(result > 0.0f);
  return result;
}

FloatSample
UnitDSP::Next(CvState const& cv)
{
  _output.Clear();
  if (!_model->on) return Output();
  ModInput mod = ModulationInput(cv, _model->src1, _model->src2);
  float freq = ModFreq(mod);
  float phase = ModPhase(mod);
  float sample = Generate(phase, freq, mod);
  float amp = Mod(UnitModTarget::Amp, _amp, false, mod);
  float pan = BipolarToUnipolar1(Mod(UnitModTarget::Pan, _pan, true, mod));
  _phase += freq / _rate;
  _phase -= std::floor(_phase);
  assert(-1.0 <= sample && sample <= 1.0);
  _output.left = sample * amp * (1.0f - pan);
  _output.right = sample * amp * pan;
  return Output();
}

float
UnitDSP::Generate(float phase, float freq, ModInput const& mod)
{
  switch (_model->type)
  {
  case UnitType::Sin: return std::sinf(phase * 2.0f * PIF);
  case UnitType::Add: return GenerateAdd(phase, freq, mod);
  case UnitType::Blep: return GenerateBlep(phase, freq, mod);
  default: assert(false); return 0.0f;
  }
}

// http://www.martin-finke.de/blog/articles/audio-plugins-018-polyblep-oscillator/
float
UnitDSP::GenerateBlep(float phase, float freq, ModInput const& mod)
{
  float result = 0.0f;
  const double BlepLeaky = 1.0e-4;
  float inc = freq / _rate;
  if (_model->blepType == BlepType::Saw)
  {
    result = GenerateBlepSaw(phase + 0.5f, inc);
    assert(-1.0f <= result && result <= 1.0f);
    return result;
  }

  float modPw = Mod(UnitModTarget::Pw, _pw, false, mod);
  float pwPhase = phase + 0.5f - modPw * 0.5f;
  pwPhase -= (int)pwPhase;
  if(_model->blepType == BlepType::Pulse)
  {
    float saw = GenerateBlepSaw(phase, inc);
    float result = (saw - GenerateBlepSaw(pwPhase, inc)) * 0.5f;
    assert(-1.0f <= result && result <= 1.0f);
    return result;
  }

  if(_model->blepType != BlepType::Tri) return assert(false), 0.0f;
  float saw = GenerateBlepSaw(phase + 0.25f, inc);
  float pulse = (saw - GenerateBlepSaw(pwPhase + 0.25f, inc)) * 0.5f;
  _blepTri = (1.0 - BlepLeaky) * _blepTri + inc * pulse;
  result = static_cast<float>(_blepTri) * (1.0f + modPw) * 4.0f;
  assert(-1.0f <= result && result <= 1.0f);
  return result;
}

float 
UnitDSP::GenerateAdd(float phase, float freq, ModInput const& mod) const
{
  bool any = false;
  float limit = 0.0;
  float result = 0.0;
  int step = _model->addStep;
  int parts = _model->addParts;
  bool addSub = _model->addSub;
  float roll = Mod(UnitModTarget::Roll, _roll, true, mod);

  __m256 psRolls;
  __m256 ones = _mm256_set1_ps(1.0f);
  __m256 zeros = _mm256_set1_ps(0.0f);
  __m256 signs = _mm256_set1_ps(1.0f);
  __m256 freqs = _mm256_set1_ps(freq);
  __m256 rolls = _mm256_set1_ps(roll);
  __m256 limits = _mm256_set1_ps(0.0f);
  __m256 results = _mm256_set1_ps(0.0f);
  __m256 phases = _mm256_set1_ps(phase);
  __m256 twopis = _mm256_set1_ps(2.0f * PIF);
  __m256 nyquists = _mm256_set1_ps(_rate / 2.0f);
  __m256 maxPs = _mm256_set1_ps(parts * static_cast<float>(step));

  if(addSub) signs = _mm256_set_ps(1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f, 1.0f, -1.0f);
  for (int p = 1; p <= parts * step; p += step * 8)
  {
    if(p * freq >= _rate * 0.5f) break;
    __m256 allPs = _mm256_set_ps(
      p + 0.0f * step, p + 1.0f * step, p + 2.0f * step, p + 3.0f * step,
	    p + 4.0f * step, p + 5.0f * step,	p + 6.0f * step, p + 7.0f * step);
    __m256 belowMax = _mm256_cmp_ps(allPs, maxPs, _CMP_LE_OQ);
	  __m256 belowNyquists = _mm256_cmp_ps(_mm256_mul_ps(allPs, freqs), nyquists, _CMP_LT_OQ);
    __m256 wantedPs = _mm256_blendv_ps(zeros, _mm256_blendv_ps(zeros, ones, belowMax), belowNyquists);
    if (roll >= 0.0f) psRolls = _mm256_mul_ps(allPs, _mm256_add_ps(ones, _mm256_mul_ps(_mm256_sub_ps(allPs, ones), rolls)));
    else psRolls = _mm256_add_ps(ones, _mm256_mul_ps(_mm256_sub_ps(allPs, ones), _mm256_add_ps(rolls, ones)));
    __m256 amps = _mm256_div_ps(ones, psRolls);
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
UnitDSP::Plot(UnitPlotState* state)
{
  if (!state->model->on) return;

  CycledPlotState cycled;
  cycled.cycles = 5;
  cycled.flags = PlotBipolar;
  cycled.input = state->input;
  cycled.output = state->output;
  if (state->spectrum) cycled.flags |= PlotSpectrum;
  cycled.frequency = Freq(*state->model, 4, UnitNote::C);

  auto next = [](std::tuple<CvDSP, UnitDSP>& state) 
  { 
    auto const& cv = std::get<CvDSP>(state).Next();
    return std::get<UnitDSP>(state).Next(cv).Mono(); 
  };

  auto factory = [&](float rate)
  { 
    UnitDSP unit(state->model, 4, UnitNote::C, rate);
    CvDSP cv(state->cvModel, 1.0f, state->input->bpm, rate);
    return std::make_tuple(cv, unit); 
  };

  PlotDSP::RenderCycled(&cycled, factory, next);
}

} // namespace Xts