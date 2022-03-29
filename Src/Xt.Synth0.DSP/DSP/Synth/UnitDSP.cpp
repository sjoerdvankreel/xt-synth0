#include <DSP/Shared/Plot.hpp>
#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/UnitDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <cmath>
#include <cassert>
#include <immintrin.h>

#define BLEP_LEAKY 1.0e-4
#define BLEP_MAX_PW 0.975f
#define FREQ_MOD_MIN_HZ 10.0f
#define FREQ_MOD_MAX_HZ 10000.0f

// http://www.martin-finke.de/blog/articles/audio-plugins-018-polyblep-oscillator/
// https://www.musicdsp.org/en/latest/Synthesis/160-phase-modulation-vs-frequency-modulation-ii.html

namespace Xts {

PeriodicParams
UnitPlot::Params() const
{
  PeriodicParams result;
  result.periods = 5;
  result.bipolar = true;
  result.autoRange = false;
  result.allowResample = true;
  return result;
}

float
UnitPlot::Frequency(float bpm, float rate) const 
{ return UnitDSP::Frequency(_model->voice.audio.units[_index], 4, UnitNote::C); }

void
UnitPlot::Init(float bpm, float rate)
{
  new(&_cvDsp) CvDSP(&_model->voice.cv, 1.0, bpm, rate);
  new(&_globalLfoDsp) LfoDSP(&_model->global.lfo, bpm, rate);
  new(&_unitDsp) UnitDSP(&_model->voice.audio.units[_index], 4, UnitNote::C, rate);
}

void
UnitPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  int base = static_cast<int>(PlotType::Unit1);
  int type = static_cast<int>(model.global.plot.type);
  UnitModel const* unit = &model.voice.audio.units[type - base];
  if (unit->on) UnitPlot(& model, type - base).DoRender(input, state);
}

static __m256
AdditivePartialRolloffs(__m256 indices, float rolloff)
{
  __m256 result;
  __m256 ones = _mm256_set1_ps(1.0f);
  __m256 rolloffs = _mm256_set1_ps(rolloff);
  if (rolloff >= 0.0f)
  {
    result = _mm256_sub_ps(indices, ones);
    result = _mm256_mul_ps(result, rolloffs);
    result = _mm256_add_ps(ones, result);
    return _mm256_mul_ps(indices, result);
  }
  result = _mm256_sub_ps(indices, ones);
  result = _mm256_mul_ps(result, _mm256_add_ps(rolloffs, ones));
  return _mm256_add_ps(ones, result);
}

static float
GeneratePolyBlepSaw(float phase, float increment)
{
  float blep;
  if (phase >= 1.0f) phase -= 1.0f;
  float saw = 2.0f * phase - 1.0f;
  if(phase < increment) return blep = phase / increment, saw - ((2.0f - blep) * blep - 1.0f);
  if(phase >= 1.0f - increment) return blep = (phase - 1.0f) / increment, saw - ((blep + 2.0f) * blep + 1.0f);
  return saw;
}

UnitDSP::
UnitDSP(UnitModel const* model, int octave, UnitNote note, float rate) :
UnitDSP()
{
  _phase = 0.0;
  _rate = rate;
  _note = note;
  _model = model;
  _octave = octave;
  _blepTriangle = 0.0;
  _output = FloatSample();
  _mods = TargetModsDSP(&model->mods);
}

float
UnitDSP::Frequency(UnitModel const& model, int octave, UnitNote note)
{
  float cent = Param::Mix(model.detune) * 0.5f;
  int key = octave * 12 + static_cast<int>(note);
  int base = 4 * 12 + static_cast<int>(UnitNote::C);
  int unit = (model.octave + 1) * 12 + static_cast<int>(model.note);
  return MidiNoteFrequency(unit + key - base + cent);
}

float
UnitDSP::ModulatePhase() const
{
  float amount1 = _mods.Mod1().Amount();
  float amount2 = _mods.Mod2().Amount();
  CvSample output1 = _mods.Mod1().Output();
  CvSample output2 = _mods.Mod2().Output();
  float phase = static_cast<float>(_phase);
  float base1 = output1.bipolar ? 0.5f : amount1 >= 0.0f ? 0.0f : 1.0f;
  float base2 = output2.bipolar ? 0.5f : amount2 >= 0.0f ? 0.0f : 1.0f;
  if (_model->mods.mod1.target == static_cast<int>(UnitModTarget::Phase)) phase += Xts::Modulate({ base1, false }, output1, amount1);
  if (_model->mods.mod2.target == static_cast<int>(UnitModTarget::Phase)) phase += Xts::Modulate({ base2, false }, output2, amount2);
  return UnipolarSanity(phase - std::floorf(phase));
}

float
UnitDSP::ModulateFrequency() const
{
  float result = Frequency(*_model, _octave, _note);
  float pitchRange = 0.02930223f; 
  float amount1 = _mods.Mod1().Amount();
  float amount2 = _mods.Mod2().Amount();
  CvSample output1 = _mods.Mod1().Output();
  CvSample output2 = _mods.Mod2().Output();
  float frequencyRange = FREQ_MOD_MAX_HZ - FREQ_MOD_MIN_HZ;
  float frequencyBase = (std::max(FREQ_MOD_MIN_HZ, std::min(result, FREQ_MOD_MAX_HZ)) - FREQ_MOD_MIN_HZ) / frequencyRange;
  if (_model->mods.mod1.target == static_cast<int>(UnitModTarget::Pitch)) result *= 1.0f + Xts::Modulate({ 0.0f, true }, output1, amount1) * pitchRange;
  if (_model->mods.mod1.target == static_cast<int>(UnitModTarget::Frequency)) result = FREQ_MOD_MIN_HZ + Xts::Modulate({ frequencyBase, false }, output1, amount1) * frequencyRange;
  frequencyBase = (std::max(FREQ_MOD_MIN_HZ, std::min(result, FREQ_MOD_MAX_HZ)) - FREQ_MOD_MIN_HZ) / frequencyRange;
  if (_model->mods.mod2.target == static_cast<int>(UnitModTarget::Pitch)) result *= 1.0f + Xts::Modulate({ 0.0f, true }, output2, amount2) * pitchRange;
  if (_model->mods.mod2.target == static_cast<int>(UnitModTarget::Frequency)) result = FREQ_MOD_MIN_HZ + Xts::Modulate({ frequencyBase, false }, output2, amount2) * frequencyRange;
  assert(result > 0.0f);
  return Sanity(result);
}

FloatSample
UnitDSP::Next(CvState const& cv)
{
  _output.Clear();
  if (!_model->on) return Output();
  _mods.Next(cv);
  float phase = ModulatePhase();
  float frequency = ModulateFrequency();
  float sample = BipolarSanity(Generate(phase, frequency));
  float ampBase = Param::Level(_model->amp);
  float amp = _mods.Modulate({ ampBase, false }, static_cast<int>(UnitModTarget::Amp));
  float panBase = Param::Mix(_model->panning);
  float panning = BipolarToUnipolar1(_mods.Modulate({ panBase, true }, static_cast<int>(UnitModTarget::Panning)));
  _phase += frequency / _rate;
  _phase -= std::floor(_phase);
  _output = { sample * amp * (1.0f - panning), sample * amp * panning };
  return Output();
}

float
UnitDSP::Generate(float phase, float frequency)
{
  switch (_model->type)
  {
  case UnitType::Sine: return std::sinf(phase * 2.0f * PIF);
  case UnitType::Additive: return GenerateAdditive(phase, frequency);
  case UnitType::PolyBlep: return GeneratePolyBlep(phase, frequency);
  default: assert(false); return 0.0f;
  }
}

float
UnitDSP::GeneratePolyBlep(float phase, float frequency)
{
  float result = 0.0f;
  float increment = frequency / _rate;
  float bpwBase = Param::Level(_model->blepPulseWidth);
  if (_model->blepType == BlepType::Saw) return BipolarSanity(GeneratePolyBlepSaw(phase + 0.5f, increment));
  float pulseWidth = _mods.Modulate({ bpwBase, false }, static_cast<int>(UnitModTarget::BlepPulseWidth));
  float phase2 = phase + 0.5f - pulseWidth * 0.5f;
  if(_model->blepType == BlepType::Pulse) return BipolarSanity((GeneratePolyBlepSaw(phase, increment) - GeneratePolyBlepSaw(phase2, increment)) * 0.5f);
  if (_model->blepType != BlepType::Triangle) return assert(false), 0.0f;
  float pulse = (GeneratePolyBlepSaw(phase + 0.25f, increment) - GeneratePolyBlepSaw(phase2 + 0.25f, increment)) * 0.5f;
  _blepTriangle = (1.0 - BLEP_LEAKY) * _blepTriangle + increment * pulse;
  return BipolarSanity(static_cast<float>(_blepTriangle) * (1.0f + pulseWidth) * 4.0f);
}

float 
UnitDSP::GenerateAdditive(float phase, float frequency) const
{
  bool any = false;
  float even = 1.0f;
  float limit = 0.0;
  float result = 0.0;
  int step = _model->additiveStep;
  int partials = _model->additivePartials;
  float odd = _model->additiveSub != 0 ? -1.0f : 1.0f;
  float rolloffBase = Param::Mix(_model->additiveRolloff);
  float rolloff = _mods.Modulate({ rolloffBase, true }, static_cast<int>(UnitModTarget::AdditiveRolloff));

  __m256 ones = _mm256_set1_ps(1.0f);
  __m256 zeros = _mm256_set1_ps(0.0f);
  __m256 limits = _mm256_set1_ps(0.0f);
  __m256 results = _mm256_set1_ps(0.0f);
  __m256 phases = _mm256_set1_ps(phase);
  __m256 twopis = _mm256_set1_ps(2.0f * PIF);
  __m256 nyquists = _mm256_set1_ps(_rate / 2.0f);
  __m256 frequencies = _mm256_set1_ps(frequency);
  __m256 maxPartials = _mm256_set1_ps(partials * static_cast<float>(step));
  __m256 signs = _mm256_set_ps(even, odd, even, odd, even, odd, even, odd);

  for (int p = 1; p <= partials * step; p += step * 8)
  {
    if(p * frequency >= _rate * 0.5f) break;
    float f0 = static_cast<float>(p);
    float f1 = static_cast<float>(p + step);
    float f2 = static_cast<float>(p + step * 2.0f);
    float f3 = static_cast<float>(p + step * 3.0f);
    float f4 = static_cast<float>(p + step * 4.0f);
    float f5 = static_cast<float>(p + step * 5.0f);
    float f6 = static_cast<float>(p + step * 6.0f);
    float f7 = static_cast<float>(p + step * 7.0f);
    __m256 indices = _mm256_set_ps(f0, f1, f2, f3, f4, f5, f6, f7);
    __m256 partialFrequencies = _mm256_mul_ps(indices, frequencies);
    __m256 partialRolloffs = AdditivePartialRolloffs(indices, rolloff);
    __m256 partialPhases = _mm256_mul_ps(phases, indices);
    __m256 partialAmps = _mm256_div_ps(ones, partialRolloffs);
    __m256 partialAngles = _mm256_mul_ps(partialPhases, twopis);
    __m256 partialSines = _mm256_sin_ps(partialAngles);
    __m256 partialWaves = _mm256_mul_ps(partialSines, partialAmps);
    __m256 partialResults = _mm256_mul_ps(partialWaves, signs);
    __m256 belowMaxMask = _mm256_cmp_ps(indices, maxPartials, _CMP_LE_OQ);
    __m256 belowMaxSelector = _mm256_blendv_ps(zeros, ones, belowMaxMask);
	  __m256 belowNyquists = _mm256_cmp_ps(partialFrequencies, nyquists, _CMP_LT_OQ);
    __m256 belowNyquistSelector = _mm256_blendv_ps(zeros, belowMaxSelector, belowNyquists);
    __m256 belowNyquistAmps = _mm256_mul_ps(partialAmps, belowNyquistSelector);
    __m256 belowNyquistResults = _mm256_mul_ps(partialResults, belowNyquistSelector);
	  limits = _mm256_add_ps(limits, belowNyquistAmps);
	  results = _mm256_add_ps(results, belowNyquistResults);
	  any = true;
  }

  for(int i = 0; i < partials && i < 8; i++)
  {
	  limit += limits.m256_f32[7 - i];
	  result += results.m256_f32[7 - i];
  }
  return BipolarSanity(any? result / limit: 0.0f);
}

} // namespace Xts