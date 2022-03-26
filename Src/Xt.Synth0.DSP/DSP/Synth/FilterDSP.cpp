#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/VoiceFilterPlot.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cstring>
#include <cassert>

#define FILTER_MIN_FREQ_HZ 20.0f
#define FILTER_MAX_FREQ_HZ 10000.0f

// https://cytomic.com/files/dsp/SvfLinearTrapOptimised2.pdf
// https://www.dsprelated.com/freebooks/filters/Analysis_Digital_Comb_Filter.html

namespace Xts {

FilterDSP::
FilterDSP(FilterModel const* model, float rate):
FilterDSP()
{
  _rate = rate;
  _model = model;
  _comb.x.Clear();
  _comb.y.Clear();
  _stateVar.ic1eq.Clear();
  _stateVar.ic2eq.Clear();
}

FloatSample
FilterDSP::GenerateStateVar(FloatSample x, int freq, double res)
{
  auto& s = _stateVar;
  float hz = Param::Frequency(freq, FILTER_MIN_FREQ_HZ, FILTER_MAX_FREQ_HZ);
  double g = std::tan(PID * hz / _rate);
  double k = 2.0 - 2.0 * res;
  double a1 = 1.0 / (1.0 + g * (g + k));
  double a2 = g * a1;
  double a3 = g * a2;
  
  auto v0 = x.ToDouble();
  auto v3 = v0 - s.ic2eq;
  auto v1 = a1 * s.ic1eq + a2 * v3;
  auto v2 = s.ic2eq + a2 * s.ic1eq + a3 * v3;
  s.ic1eq = 2.0 * v1 - s.ic1eq;
  s.ic2eq = 2.0 * v2 - s.ic2eq;

  DoubleSample result = {};
  switch (_model->passType)
  {
  case PassType::LPF: result = v2; break;
  case PassType::BPF: result = v1; break;
  case PassType::BSF: result = v0 - k * v1; break;
  case PassType::HPF: result = v0 - k * v1 - v2; break;
  default: assert(false); break;
  }
  return result.ToFloat().Sanity();
}

FloatSample
FilterDSP::GenerateComb(FloatSample x, int minDelay, int plusDelay, float minGain, float plusGain)
{
  auto& s = _comb;
  int minDelaySamples = static_cast<int>(Param::TimeSamplesF(minDelay, _rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  int plusDelaySamples = static_cast<int>(Param::TimeSamplesF(plusDelay, _rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  s.y.Push(x + s.x.Get(plusDelaySamples) * plusGain + s.y.Get(minDelaySamples) * minGain);
  s.x.Push(x);
  assert(minDelaySamples < COMB_DELAY_MAX_SAMPLES);
  assert(plusDelaySamples < COMB_DELAY_MAX_SAMPLES);
  return s.y.Get(0).Sanity();
}

} // namespace Xts