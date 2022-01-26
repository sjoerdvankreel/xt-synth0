#include "DSP.hpp"
#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Release()
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Release();
}

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
  int hold;
  int h = 0;
  bool l = output.channel == 0;
  auto env = static_cast<int>(model.global.ampEnv);
  env -= static_cast<int>(GlobalAmpEnv::Env1);

  output.bipolar = true;
  EnvDSP::PlotParams(model.envs[env], input, output.rate, hold);
  SynthInput in(output.rate, input.bpm, 4, UnitNote::C);
  SynthDSP dsp(&model, &in);
  while (true)
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