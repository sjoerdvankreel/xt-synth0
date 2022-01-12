#include "PlotDSP.hpp"
#include <cassert>
#include <cmath>

namespace Xts {

void 
PlotDSP::Render(
  SynthModel const& synth, int32_t pixels, int32_t* rate, XtsBool* bipolar, 
  float* frequency, float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount)
{
  _splits.clear();
  _samples.clear();
  *frequency = 0.0f;
  *bipolar = XtsFalse;
  PlotFit fit = static_cast<PlotFit>(synth.plot.fit);
  switch(static_cast<PlotSource>(synth.plot.source))
  {
  case PlotSource::Env1: RenderEnv(synth.envs[0], pixels, fit, rate, bipolar); break;
  case PlotSource::Env2: RenderEnv(synth.envs[1], pixels, fit, rate, bipolar); break;
  case PlotSource::Unit1: RenderUnit(synth.units[0], pixels, fit, rate, bipolar, frequency); break;
  case PlotSource::Unit2: RenderUnit(synth.units[1], pixels, fit, rate, bipolar, frequency); break;
  case PlotSource::Unit3: RenderUnit(synth.units[2], pixels, fit, rate, bipolar, frequency); break;
  default: assert(false); break;
  }
  *splits = _splits.data();
  *samples = _samples.data();
  *splitCount = static_cast<int32_t>(_splits.size());
  *sampleCount = static_cast<int32_t>(_samples.size());
}

void
PlotDSP::RenderUnit(
  UnitModel const& unit, int32_t pixels, PlotFit fit, 
  int32_t* rate, XtsBool* bipolar, float* frequency)
{
  bool cycled;
  _unit.Reset();
  *bipolar = XtsTrue;
  const float cycleCount = 1.5f;
  bool doFit = fit == PlotFit::Fit;
  *frequency = _unit.Frequency(unit);
  if (doFit) *rate = static_cast<int32_t>(ceilf(*frequency * pixels / cycleCount));
  float cycleLength = *rate / *frequency;
  float ratef = static_cast<float>(*rate);
  int samples = static_cast<int>(cycleCount * cycleLength);
  for (int i = 0; i <= samples; i++)
  {
    _samples.push_back(_unit.Next(unit, ratef, &cycled));
    if (cycled) _splits.push_back(i + 1);
  }
}

void
PlotDSP::RenderEnv(
  EnvModel const& env, int32_t pixels, 
  PlotFit fit, int32_t* rate, XtsBool* bipolar)
{
  _env.Reset();
  int sample = 0;
  *bipolar = XtsFalse;
  int activeSamples = 0;
  float dly, a, hld, d, r;
  const int maxSamples = 96000;
  const int minHoldSamples = 10;
  EnvStage stage = EnvStage::Dly;
  EnvStage prevStage = EnvStage::Dly;
  const float holdFactor = 1.0f / 3.0f;
  bool doFit = fit != PlotFit::Rate;
  float ratef = static_cast<float>(*rate);
  _env.Length(env, ratef, &dly, &a, &hld, &d, &r);
  float envSamples = dly + a + hld + d + r;
  int holdSamples = static_cast<int>(envSamples * holdFactor);
  holdSamples = std::max(holdSamples, minHoldSamples);
  float totalSamples = holdSamples + envSamples;
  if (doFit)
  {
    *rate = static_cast<int32_t>(*rate / totalSamples * pixels);
    RenderEnv(env, pixels, PlotFit::Rate, rate, bipolar);
    return;
  }
  else if (totalSamples > maxSamples)
  {
    *rate = static_cast<int32_t>(*rate / totalSamples * maxSamples);
    RenderEnv(env, pixels, PlotFit::Rate, rate, bipolar);
    return;
  }
  while (true)
  {
    float lvl = _env.Next(env, ratef, activeSamples < holdSamples, &stage);
    if (stage == EnvStage::End) break;
    _samples.push_back(lvl);
    if (stage == EnvStage::S) activeSamples++;
    if (sample != 0 && stage != prevStage && stage != EnvStage::End)
      _splits.push_back(sample + 1);
    prevStage = stage;
    sample++;
  }
}

} // namespace Xts