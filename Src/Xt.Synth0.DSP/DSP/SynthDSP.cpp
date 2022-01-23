#include "DSP.hpp"
#include "SynthDSP.hpp"

namespace Xts {

SynthDSP::
SynthDSP(SynthModel const* model, SynthInput const* input):
DSPBase(model, input), _global(&model->global, input), _envs(), _units()
{
  for (int l = 0; l < LfoCount; l++)
    _lfos[l] = LfoDSP(&model->lfos[l], input);
  for (int e = 0; e < EnvCount; e++)
    _envs[e] = EnvDSP(&model->envs[e], input);
  for (int u = 0; u < UnitCount; u++)
    _units[u] = UnitDSP(&model->units[u], input);
}

void
SynthDSP::Release()
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Release();
}

AudioOutput
SynthDSP::Next(SynthState const& state)
{
  AudioOutput output;
  for (int u = 0; u < UnitCount; u++)
    output += _units[u].Next(state);
  return output * _global.Amp(state);
}

AudioOutput
SynthDSP::Next()
{
  SynthState state;
  if (End()) return AudioOutput(0.0f, 0.0f);
  for (int l = 0; l < LfoCount; l++)
    state.lfos[l] = _lfos[l].Next();
  for (int e = 0; e < EnvCount; e++)
    state.envs[e] = _envs[e].Next();
  return Next(state);
}

void
SynthDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  const int plotRate = 5000;
  const int plotMaxSamples = 10000;
  bool l = output.channel == 0;

  output.freq = 0.0f;
  output.clip = false;
  output.bipolar = true;
  output.rate = static_cast<float>(plotRate); 
  
  SynthInput in(output.rate, 120, 4, UnitNote::C);
  SynthDSP dsp(&model, &in);
  for(int s = 0; s < plotMaxSamples; s++)
  {
    if(dsp.End()) break;
    auto audio = dsp.Next();
    float sample = l? audio.l: audio.r;
    output.clip |= Clip(sample);
    output.samples->push_back(sample);
  }
}

} // namespace Xts