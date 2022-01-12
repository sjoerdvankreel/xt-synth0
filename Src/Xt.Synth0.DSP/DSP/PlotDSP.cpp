#include "PlotDSP.hpp"
#include <cassert>
#include <cmath>

namespace Xts {

void
PlotDSP::RenderEnv(
  EnvModel const& env, int32_t pixels, bool fit, int32_t* rate)
{
  _env.Reset();
  int sample = 0;
  bool active = true;
  int activeSamples = 0;
  float dly, a, hld, d, r;
  const int maxSamples = 96000;
  EnvStage stage = EnvStage::Dly;
  EnvStage prevStage = EnvStage::Dly;
  const float holdFactor = 1.0f / 3.0f;
  float ratef = static_cast<float>(*rate);
  _env.Length(env, ratef, &dly, &a, &hld, &d, &r);
  float envSamples = dly + a + hld + d + r;
  int holdSamples = static_cast<int>(envSamples * holdFactor);
  float totalSamples = holdSamples + envSamples;
  if(fit || totalSamples > maxSamples)
  {
    *rate = static_cast<int32_t>(*rate * totalSamples / pixels);
    RenderEnv(env, pixels, false, rate);
  }
  while(stage != EnvStage::End)
  {
    _samples.push_back(_env.Next(env, ratef, active, &stage));
    if(stage == EnvStage::S)
    {
      activeSamples++;
      if(activeSamples >= holdSamples)
        active = false;
    }
    if(sample != 0 && stage != prevStage && stage != EnvStage::End)
      _splits.push_back(sample + 1);
    prevStage = stage;
    sample++;
  }
}

void 
PlotDSP::RenderUnit(
  UnitModel const& unit, int32_t pixels, bool fit, int32_t* rate, float* frequency)
{
  bool cycled;
  _unit.Reset();
  const float cycleCount = 1.5f;
  *frequency = _unit.Frequency(unit);
  if(fit) *rate = static_cast<int32_t>(ceilf(*frequency * pixels / cycleCount));
  float cycleLength = *rate / *frequency;
  float ratef = static_cast<float>(*rate);
  int samples = static_cast<int>(cycleCount * cycleLength);
  for(int i = 0; i <= samples; i++)
  {
    _samples.push_back(_unit.Next(unit, ratef, &cycled));
    if(cycled) _splits.push_back(i + 1);
  }
}

void 
PlotDSP::Render(
  SynthModel const& synth, int32_t pixels, int32_t* rate, float* frequency,
  float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount)
{
  _splits.clear();
  _samples.clear();
  *frequency = 0.0f;
  bool fit = synth.global.fitPlot != 0;
  switch(static_cast<PlotType>(synth.global.plot))
  {
  case PlotType::Env1: RenderEnv(synth.envs[0], pixels, fit, rate); break;
  case PlotType::Env2: RenderEnv(synth.envs[1], pixels, fit, rate); break;
  case PlotType::Unit1: RenderUnit(synth.units[0], pixels, fit, rate, frequency); break;
  case PlotType::Unit2: RenderUnit(synth.units[1], pixels, fit, rate, frequency); break;
  case PlotType::Unit3: RenderUnit(synth.units[2], pixels, fit, rate, frequency); break;
  default: assert(false); break;
  }
  *splits = _splits.data();
  *samples = _samples.data();
  *splitCount = static_cast<int32_t>(_splits.size());
  *sampleCount = static_cast<int32_t>(_samples.size());
}

} // namespace Xts