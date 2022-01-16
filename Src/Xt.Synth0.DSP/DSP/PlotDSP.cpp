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
  auto const& plot = input.synth->plot;
  auto fit = static_cast<PlotFit>(plot.fit);
  switch(static_cast<PlotSource>(plot.source))
  {
  case PlotSource::Unit1: RenderUnit(input, 0, output); break;
  case PlotSource::Unit2: RenderUnit(input, 1, output); break;
  case PlotSource::Unit3: RenderUnit(input, 2, output); break;
  case PlotSource::Env1: RenderEnv(input, 0, fit, input.rate, output); break;
  case PlotSource::Env2: RenderEnv(input, 1, fit, input.rate, output); break;
  case PlotSource::Global: RenderGlobal(input, fit, input.rate, output); break;
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
  auto& dsp = _dsp._units[0];
  dsp.Init(4, UnitNote::C);
  float freq = dsp.Freq(input.synth->units[index]);
  bool doFit = input.synth->plot.fit == static_cast<int>(PlotFit::Fit);
  if (doFit) rate = static_cast<int32_t>(ceilf(freq * input.pixels / cycleCount));

  float cycleLength = rate / freq;
  float ratef = static_cast<float>(rate);
  int samples = static_cast<int>(cycleCount * cycleLength);
  if(input.synth->units[index].type != static_cast<int>(UnitType::Off))
    for (int i = 0; i <= samples; i++)
    {
      dsp.Next(input.synth->units[index], ratef, uout);
      _samples.push_back(uout.l + uout.r);
      if (uout.cycled) _splits.push_back(i + 1);
    }
  output.rate = rate;
  output.freq = freq;
  output.bipolar = XtsTrue;
}

void
PlotDSP::RenderEnv(PlotInput const& input, int index, PlotFit fit, int32_t rate, PlotOutput& output)
{
  const int maxSamples = 96000;
  const int minHoldSamples = 10;
  const float holdFactor = 1.0f / 3.0f;

  auto& dsp = _dsp._envs[0];
  dsp.Init();
  int sample = 0;
  int sustainSamples = 0;
  float ratef = static_cast<float>(rate);
  EnvParams params = dsp.Params(input.synth->envs[index], ratef);
  float length = dsp.Frames(params);
  int holdSamples = std::max(static_cast<int>(length * holdFactor), minHoldSamples);
  float totalSamples = holdSamples + length;
  
  if (fit != PlotFit::Rate)
  {
    rate = static_cast<int32_t>(rate / totalSamples * input.pixels);
    RenderEnv(input, index, PlotFit::Rate, rate, output);
    return;
  }
  if (totalSamples > maxSamples) 
  {
    rate = static_cast<int32_t>(rate / totalSamples * maxSamples);
    RenderEnv(input, index, PlotFit::Rate, rate, output);
    return;
  }

  EnvOutput eout;
  ratef = static_cast<float>(rate);
  while (input.synth->envs[index].type != static_cast<int>(EnvType::Off))
  {
    if(sustainSamples == holdSamples) dsp.Release();
    dsp.Next(input.synth->envs[index], ratef, eout);
    if (eout.stage == EnvStage::End) break;
    _samples.push_back(eout.lvl);
    if (eout.stage == EnvStage::S) sustainSamples++;
    if (sample != 0 && eout.staged) _splits.push_back(sample);
    sample++;
  }
  output.rate = rate;
  output.bipolar = XtsFalse;
}

void
PlotDSP::RenderGlobal(PlotInput const& input, PlotFit fit, int32_t rate, PlotOutput& output)
{
  const int minRate = 10000;
  const int maxSamples = 96000;

  EnvParams params {};
  float samples = 0.0f;
  int sustainSamples = 0;
  _dsp.Init(4, UnitNote::C);
  const float holdFactor = 1.0f / 3.0f;
  float ratef = static_cast<float>(rate);
  auto env = static_cast<AmpEnv>(input.synth->global.env);
  auto length = [&](int index)
  {
    auto& envDsp = _dsp._envs[index];
    auto params = envDsp.Params(input.synth->envs[index], ratef);
    return envDsp.Frames(params);
  };
  switch (env)
  {
  case AmpEnv::AmpEnv1: samples = length(0); break;
  case AmpEnv::AmpEnv2: samples = length(1); break;
  case AmpEnv::NoAmpEnv: samples = maxSamples; break;
  default: assert(false); break;
  }

  if (fit != PlotFit::Rate)
  {
    float newRate = rate / samples * input.pixels;
    rate = std::max(minRate, static_cast<int32_t>(newRate));
    RenderGlobal(input, PlotFit::Rate, rate, output);
    return;
  }

  SynthOutput sout;
  samples = static_cast<float>(std::min(maxSamples, static_cast<int>(samples)));
  ratef = static_cast<float>(rate);
  for(int i = 0; i < samples; i++)
  {
    _dsp.Next(*input.synth, ratef, sout);
    _samples.push_back(sout.l + sout.r);
    if(env == AmpEnv::AmpEnv1 && sout.envs[0].stage == EnvStage::S) _dsp.Release();
    if(env == AmpEnv::AmpEnv2 && sout.envs[1].stage == EnvStage::S) _dsp.Release();
  }
  output.rate = rate;
  output.bipolar = XtsTrue;
}

} // namespace Xts