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
SynthDSP(SynthModel const* model, AudioInput const* input):
GeneratorDSP(model, input), _envs(), _units()
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e] = EnvDSP(&model->envs[e], input);
  for (int u = 0; u < UnitCount; u++)
    _units[u] = UnitDSP(&model->units[u], input);
}

AudioOutput
SynthDSP::Next()
{
  AudioOutput output;
  float envs[EnvCount];
  if (End()) return AudioOutput(0.0f, 0.0f);
  for(int e = 0; e < EnvCount; e++)
    envs[e] = _envs[e].Next();
  float env = envs[0] * Level(_model->global.env1);
  float amp = _model->global.amp + (1.0f - _model->global.amp) * env;
  for (int u = 0; u < UnitCount; u++)
    output += _units[u].Next() * amp;
  return output;
}

void
SynthDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  bool clip;
  output.freq = 0.0f;
  output.clip = false;
  output.bipolar = true;
  output.rate = input.pixels;
  
  AudioInput in(output.rate, 120, 4, UnitNote::C);
  SynthDSP dsp(&model, &in);
  for(int s = 0; s <= static_cast<int>(output.rate); s++)
  {
    auto sample = dsp.Next();
    sample.l = Clip(sample.l, clip);
    output.clip |= clip;
    sample.r = Clip(sample.r, clip);
    output.clip |= clip;
    output.samples->push_back(sample.Mono());
  }
}

} // namespace Xts