#include "DSP.hpp"
#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Init(SynthModel const& model, AudioInput const& input)
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Init(model.envs[e], input);
  for (int u = 0; u < UnitCount; u++)
    _units[u].Init(model.units[u], input);
}

void
SynthDSP::Release(SynthModel const& model, AudioInput const& input)
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Release(model.envs[e], input);
  for (int u = 0; u < UnitCount; u++)
    _units[u].Release(model.units[u], input);
}

AudioOutput
SynthDSP::Next(SynthModel const& model, AudioInput const& input)
{
  AudioOutput output;
  float envs[EnvCount];
  if (End()) return AudioOutput(0.0f, 0.0f);
  for(int e = 0; e < EnvCount; e++)
    envs[e] = _envs[e].Next(model.envs[e], input);
  float env = envs[0] * Level(model.global.env1);
  float amp = model.global.amp + (1.0 - model.global.amp) * env;
  for (int u = 0; u < UnitCount; u++)
    output += _units[u].Next(model.units[u], input) * amp;
  return output;
}

void
SynthDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  output.freq = 0.0f;
  output.bipolar = true;
  output.rate = input.pixels;
  
  AudioInput in(output.rate, 120, 4, UnitNote::C);
  Init(model, in);
  for(int s = 0; s <= static_cast<int>(output.rate); s++)
    output.samples->push_back(Next(model, in).Mono());
}

} // namespace Xts