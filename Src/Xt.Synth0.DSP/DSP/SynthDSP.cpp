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
  const int plotRate = 5000;
  const int plotMaxSamples = 10000;

  output.freq = 0.0f;
  output.clip = false;
  output.bipolar = true;
  output.rate = static_cast<float>(plotRate);
  
  AudioInput in(output.rate, 120, 4, UnitNote::C);
  SynthDSP dsp(&model, &in);
  for(int s = 0; s < plotMaxSamples; s++)
  {
    if(dsp.End()) break;
    float sample = dsp.Next().l;
    output.clip |= Clip(sample);
    output.samples->push_back(sample);
  }
}

} // namespace Xts