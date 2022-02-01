#include "DSP.hpp"
#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Release()
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Release();
}

void
SynthDSP::Next(SynthState const& state)
{
  AudioOutput output;
  for (int u = 0; u < UnitCount; u++)
  {
    _units[u].Next(state);
    output += _units[u].Value();
  }
  _value = output * _global.Amp(state);
}

AudioOutput
SynthDSP::Next()
{
  SynthState state;
  if (End()) return AudioOutput(0.0f, 0.0f);
  for (int l = 0; l < LfoCount; l++)
  {
    _lfos[l].Next();
    state.lfos[l] = _lfos[l].Value();
  }
  for (int e = 0; e < EnvCount; e++)
  {
    _envs[e].Next();
    state.envs[e] = _envs[e].Value();
  }
  Next(state);
  return Value();
}

SynthDSP::
SynthDSP(SynthModel const* model, AudioInput const* input):
DSPBase(model, input), _global(&model->global), _envs(), _units()
{
  for (int u = 0; u < UnitCount; u++)
    _units[u] = UnitDSP(&model->units[u], input);
  for (int l = 0; l < LfoCount; l++)
    _lfos[l] = LfoDSP(&model->lfos[l], &input->source);
  for (int e = 0; e < EnvCount; e++)
    _envs[e] = EnvDSP(&model->envs[e], &input->source);
}

void
SynthDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  const int plotRate = 5000;
  const int maxSamples = 5 * plotRate;

  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  int hold = TimeI(input.hold, plotRate);
  auto env = static_cast<int>(model.global.ampEnv);
  env -= static_cast<int>(GlobalAmpEnv::Env1);
  
  output.bipolar = true;
  output.rate = plotRate;
  KeyInput key(4, UnitNote::C);
  SourceInput source(plotRate, input.bpm);
  AudioInput audio(source, key);
  SynthDSP dsp(&model, &audio);
  while (i++ < maxSamples)
  {
    if (h++ == hold) dsp.Release();
    if (dsp.End()) break;
    auto audio = dsp.Next();
    float sample = l ? audio.l : audio.r;
    output.clip |= Clip(sample);
    output.samples->push_back(sample);
  }
}

} // namespace Xts