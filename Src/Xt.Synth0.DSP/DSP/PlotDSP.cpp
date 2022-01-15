#include "PlotDSP.hpp"
#include <cassert>
#include <cmath>

namespace Xts {

void 
PlotDSP::Render(PlotInput const& input, PlotOutput& output)
{
  _splits.clear();
  _samples.clear();
  output.freq = 0.0f;
  output.bipolar = XtsFalse;
  switch(static_cast<PlotSource>(input.synth->plot.source))
  {
  case PlotSource::Env1: RenderEnv(input, 0, output); break;
  case PlotSource::Env2: RenderEnv(input, 1, output); break;
  case PlotSource::Unit1: RenderUnit(input, 0, output); break;
  case PlotSource::Unit2: RenderUnit(input, 1, output); break;
  case PlotSource::Unit3: RenderUnit(input, 2, output); break;
  default: assert(false); break;
  }
  output.splits = _splits.data();
  output.samples = _samples.data();
  output.splitCount = static_cast<int32_t>(_splits.size());
  output.sampleCount = static_cast<int32_t>(_samples.size());
}

void
PlotDSP::RenderUnit(PlotInput const& input, int index, PlotOutput& output)
{
  const float cycleCount = 1.5f;

  UnitOutput uout;
  int32_t rate = input.rate;
  _unit.Init(4, UnitNote::C);
  float freq = _unit.Freq(input.synth->units[index]);
  bool doFit = input.synth->plot.fit == static_cast<int>(PlotFit::Fit);
  if (doFit) rate = static_cast<int32_t>(ceilf(freq * input.pixels / cycleCount));

  float cycleLength = rate / freq;
  float ratef = static_cast<float>(rate);
  int samples = static_cast<int>(cycleCount * cycleLength);
  for (int i = 0; i <= samples; i++)
  {
    _unit.Next(input.synth->units[index], ratef, uout);
    _samples.push_back(uout.l + uout.r);
    if (uout.cycled) _splits.push_back(i + 1);
  }
  output.rate = rate;
  output.freq = freq;
  output.bipolar = XtsTrue;
}

void
PlotDSP::RenderEnv(PlotInput const& input, int index, PlotOutput& output)
{
  const int maxSamples = 96000;
  const int minHoldSamples = 10;
  const float holdFactor = 1.0f / 3.0f;

  _env.Init();
  int sample = 0;
  int sustainSamples = 0;
  int32_t rate = input.rate;
  EnvParams params = _env.Params(input.synth->envs[index], input.rate);
  bool doFit = input.synth->plot.fit != static_cast<int>(PlotFit::Rate);
  float length = params.dly + params.a + params.hld + params.d + params.r;
  int holdSamples = std::max(static_cast<int>(length * holdFactor), minHoldSamples);
  float totalSamples = holdSamples + length;
  if (doFit) rate = static_cast<int32_t>(rate / totalSamples * input.pixels);
  if (totalSamples > maxSamples) rate = static_cast<int32_t>(rate / totalSamples * maxSamples);

  EnvOutput eout;
  float ratef = static_cast<float>(rate);
  while (true)
  {
    if(sustainSamples == holdSamples) _env.Release();
    _env.Next(input.synth->envs[index], ratef, eout);
    if (eout.stage == EnvStage::End) break;
    _samples.push_back(eout.lvl);
    if (eout.stage == EnvStage::S) sustainSamples++;
    if (sample != 0 && eout.staged) _splits.push_back(sample);
    sample++;
  }
  output.rate = rate;
  output.bipolar = XtsFalse;
}

} // namespace Xts