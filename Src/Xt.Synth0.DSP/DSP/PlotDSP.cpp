#include "PlotDSP.hpp"
#include "DSP.hpp"
#include <cassert>
#include <cmath>

namespace Xts {

void 
PlotDSP::Render(PlotInput const& input, PlotOutput0& output)
{
  _splits.clear();
  _samples.clear();
  output.freq = 0.0f;
  output.clip = XtsFalse;
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
PlotDSP::RenderUnit(PlotInput const& input, int index, PlotOutput0& output)
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
PlotDSP::RenderEnv(PlotInput const& input, int index, PlotFit fit, int32_t rate, PlotOutput0& output)
{
  const int maxSamples = 96000;
  const int minSustainSamples = 10;
  const float sustainFactor = 1.0f / 3.0f;

  auto& dsp = _dsp._envs[0];
  dsp.Init();
  int sample = 0;
  int bpm = input.synth->global.bpm;
  float ratef = static_cast<float>(rate);
  EnvParams params = dsp.Params(input.synth->envs[index], ratef, bpm);
  EnvType type = static_cast<EnvType>(input.synth->envs[index].type);
  float length = dsp.Frames(params);
  int sustainSamples = std::max(static_cast<int>(length * sustainFactor), minSustainSamples);
  float totalSamples = length + (type == EnvType::DAHDSR? sustainSamples : 0);
  
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
  int sustained = 0;
  ratef = static_cast<float>(rate);
  while (input.synth->envs[index].type != static_cast<int>(EnvType::Off))
  {
    if(sustained == sustainSamples) dsp.Release();
    dsp.Next(input.synth->envs[index], ratef, bpm, eout);
    if (eout.stage == EnvStage::End) break;
    _samples.push_back(eout.lvl);
    if (eout.stage == EnvStage::S) sustained++;
    if (sample != 0 && eout.staged) _splits.push_back(sample);
    sample++;
  }
  output.rate = rate;
  output.bipolar = XtsFalse;
}

void
PlotDSP::RenderGlobal(PlotInput const& input, PlotFit fit, int32_t rate, PlotOutput0& output)
{
  const int minRate = 10000;
  const int maxSamples = 96000;
  const float sustainFactor = 1.0f / 5.0f;

  float samples = 0.0f;
  _dsp.Init(4, UnitNote::C);
  int bpm = input.synth->global.bpm;
  float ratef = static_cast<float>(rate);
  auto params = _dsp._envs[0].Params(input.synth->envs[0], ratef, bpm);
  samples = _dsp._envs[0].Frames(params);

  if (fit != PlotFit::Rate)
  {
    float newRate = rate / samples * input.pixels;
    rate = std::max(minRate, static_cast<int32_t>(newRate));
    RenderGlobal(input, PlotFit::Rate, rate, output);
    return;
  }

  bool clip;
  SynthOutput sout;
  int sustained = 0;
  int dahdsr = static_cast<int>(EnvType::DAHDSR);
  bool sustain = input.synth->envs[0].type == dahdsr;
  int sustainSamples = static_cast<int>(samples * sustainFactor);
  samples += sustain? sustainSamples: 0;
  samples = static_cast<float>(std::min(maxSamples, static_cast<int>(samples)));
  ratef = static_cast<float>(rate);
  for(int i = 0; i < samples; i++)
  {
    if (sustained == sustainSamples) _dsp._envs[0].Release();
    _dsp.Next(*input.synth, ratef, sout);
    float sample = Clip((sout.l + sout.r) / 2.0f, clip);
    output.clip |= clip? XtsTrue: XtsFalse;
    _samples.push_back(sample);
    if (sout.envs[0].stage == EnvStage::S) sustained++;
  }
  output.rate = rate;
  output.bipolar = XtsTrue;
}

} // namespace Xts