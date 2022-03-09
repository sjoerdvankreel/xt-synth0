#include <DSP/Synth/UnitDSP.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>

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
Frequency(UnitModel const& model, int octave, UnitNote note)
{
  float cent = Param::Mix(model.detune) * 0.5f;
  int key = octave * 12 + static_cast<int>(note);
  int base = 4 * 12 + static_cast<int>(UnitNote::C);
  int unit = (model.octave + 1) * 12 + static_cast<int>(model.note);
  return MidiNoteFrequency(unit + key - base + cent);
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
  _model = model;
  _blepTriangle = 0.0;
  _output = FloatSample();
  _mod1 = ModDSP(model->mod1);
  _mod2 = ModDSP(model->mod2);
  _amp = Param::Level(model->amp);
  _panning = Param::Mix(model->panning);
  _frequency = Frequency(*model, octave, note);
  _blepPulseWidth = Param::Level(model->blepPulseWidth);
  _additiveRolloff = Param::Mix(model->additiveRolloff);
}

float
UnitDSP::Modulate(UnitModTarget target, CvSample carrier, CvSample modulator1, CvSample modulator2) const
{
  if (_model->mod1.target == target) carrier.value = _mod1.Modulate(carrier, modulator1);
  if (_model->mod2.target == target) carrier.value = _mod2.Modulate(carrier, modulator2);
  return carrier.value;
}

float
UnitDSP::ModulatePhase(CvSample modulator1, CvSample modulator2) const
{
  float phase = static_cast<float>(_phase);
  float base1 = modulator1.bipolar ? 0.5f : _mod1.Amount() >= 0.0f ? 0.0f : 1.0f;
  float base2 = modulator2.bipolar ? 0.5f : _mod2.Amount() >= 0.0f ? 0.0f : 1.0f;
  if (_model->mod1.target == UnitModTarget::Phase) phase += _mod1.Modulate({ base1, false }, modulator1);
  if (_model->mod2.target == UnitModTarget::Phase) phase += _mod2.Modulate({ base2, false }, modulator2);
  return UnipolarSanity(phase - std::floorf(phase));
}

float
UnitDSP::ModulateFrequency(CvSample modulator1, CvSample modulator2) const
{
  float result = _frequency;
  float pitchRange = 0.02930223f;
  float frequencyRange = FREQ_MOD_MAX_HZ - FREQ_MOD_MIN_HZ;
  float frequencyBase = (std::max(FREQ_MOD_MIN_HZ, std::min(result, FREQ_MOD_MAX_HZ)) - FREQ_MOD_MIN_HZ) / frequencyRange;
  if (_model->mod1.target == UnitModTarget::Pitch) result *= 1.0f + _mod1.Modulate({ 0.0f, true }, modulator1) * pitchRange;
  if (_model->mod1.target == UnitModTarget::Frequency) result = FREQ_MOD_MIN_HZ + _mod1.Modulate({ frequencyBase, false }, modulator1) * frequencyRange;
  frequencyBase = (std::max(FREQ_MOD_MIN_HZ, std::min(result, FREQ_MOD_MAX_HZ)) - FREQ_MOD_MIN_HZ) / frequencyRange;
  if (_model->mod2.target == UnitModTarget::Pitch) result *= 1.0f + _mod2.Modulate({ 0.0f, true }, modulator2) * pitchRange;
  if (_model->mod2.target == UnitModTarget::Frequency) result = FREQ_MOD_MIN_HZ + _mod2.Modulate({ frequencyBase, false }, modulator2) * frequencyRange;
  assert(result > 0.0f);
  return Sanity(result);
}

FloatSample
UnitDSP::Next(CvState const& cv)
{
  _output.Clear();
  if (!_model->on) return Output();
  CvSample modulator1 = _mod1.Modulator(cv);
  CvSample modulator2 = _mod2.Modulator(cv);
  float phase = ModulatePhase(modulator1, modulator2);
  float frequency = ModulateFrequency(modulator1, modulator2);
  float sample = BipolarSanity(Generate(phase, frequency, modulator1, modulator2));
  float amp = Modulate(UnitModTarget::Amp, { _amp, false }, modulator1, modulator2);
  float panning = BipolarToUnipolar1(Modulate(UnitModTarget::Panning, { _panning, true }, modulator1, modulator2));
  _phase += frequency / _rate;
  _phase -= std::floor(_phase);
  _output = { sample * amp * (1.0f - panning), sample * amp * panning };
  return Output();
}

float
UnitDSP::Generate(float phase, float frequency, CvSample modulator1, CvSample modulator2)
{
  switch (_model->type)
  {
  case UnitType::Sine: return std::sinf(phase * 2.0f * PIF);
  case UnitType::Additive: return GenerateAdditive(phase, frequency, modulator1, modulator2);
  case UnitType::PolyBlep: return GeneratePolyBlep(phase, frequency, modulator1, modulator2);
  default: assert(false); return 0.0f;
  }
}

float
UnitDSP::GeneratePolyBlep(float phase, float frequency, CvSample modulator1, CvSample modulator2)
{
  float result = 0.0f;
  float increment = frequency / _rate;
  if (_model->blepType == BlepType::Saw) return BipolarSanity(GeneratePolyBlepSaw(phase + 0.5f, increment));
  float pulseWidth = Modulate(UnitModTarget::BlepPulseWidth, { _blepPulseWidth, false }, modulator1, modulator2);
  float phase2 = phase + 0.5f - pulseWidth * 0.5f;
  if(_model->blepType == BlepType::Pulse) return BipolarSanity((GeneratePolyBlepSaw(phase, increment) - GeneratePolyBlepSaw(phase2, increment)) * 0.5f);
  if (_model->blepType != BlepType::Triangle) return assert(false), 0.0f;
  float pulse = (GeneratePolyBlepSaw(phase + 0.25f, increment) - GeneratePolyBlepSaw(phase2 + 0.25f, increment)) * 0.5f;
  _blepTriangle = (1.0 - BLEP_LEAKY) * _blepTriangle + increment * pulse;
  return BipolarSanity(static_cast<float>(_blepTriangle) * (1.0f + pulseWidth) * 4.0f);
}

float 
UnitDSP::GenerateAdditive(float phase, float frequency, CvSample modulator1, CvSample modulator2) const
{
  bool any = false;
  float even = 1.0f;
  float limit = 0.0;
  float result = 0.0;
  int step = _model->additiveStep;
  int partials = _model->additivePartials;
  float odd = _model->additiveSub != 0 ? -1.0f : 1.0f;
  float rolloff = Modulate(UnitModTarget::AdditiveRolloff, { _additiveRolloff, true }, modulator1, modulator2);

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
  cycled.frequency = Frequency(*state->model, 4, UnitNote::C);

  auto next = [](std::tuple<CvDSP, UnitDSP>& state) { return std::get<UnitDSP>(state).Next(std::get<CvDSP>(state).Next()).Mono(); };
  auto factory = [&](float rate) { return std::make_tuple(CvDSP(state->cv, 1.0f, state->input->bpm, rate), UnitDSP(state->model, 4, UnitNote::C, rate)); };
  PlotDSP::RenderCycled(&cycled, factory, next);
}

} // namespace Xts