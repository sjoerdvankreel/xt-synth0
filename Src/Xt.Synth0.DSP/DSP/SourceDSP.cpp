#include "SourceDSP.hpp"

namespace Xts {

void
SourceDSP::Release()
{
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Release();
}

void
SourceDSP::Next()
{
  for (int l = 0; l < LfoCount; l++)
    _lfos[l].Next();
  for (int e = 0; e < EnvCount; e++)
    _envs[e].Next();
  _value = this;
}

SourceDSP::
SourceDSP(SourceModel const* model, AudioInput const* input):
DSPBase(model, input), _lfos(), _envs()
{
  for (int l = 0; l < LfoCount; l++)
    _lfos[l] = LfoDSP(&model->lfos[l], input->source.bpm, input->source.rate);
  for (int e = 0; e < EnvCount; e++)
    _envs[e] = EnvDSP(&model->envs[e], input->source.bpm, input->source.rate);
}

} // namespace Xts