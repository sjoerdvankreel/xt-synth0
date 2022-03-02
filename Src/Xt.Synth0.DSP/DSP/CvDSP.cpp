#include "CvDSP.hpp"

namespace Xts {

EnvelopeSample
CvDSP::ReleaseAll(int env)
{ 
  for (int i = 0; i < XTS_SYNTH_ENV_COUNT; i++)
    _envs[i].Release();
  return _envs[env].Output();
}

CvState const&
CvDSP::Next()
{
  for (int i = 0; i < XTS_SYNTH_LFO_COUNT; i++)
    _output.lfos[i] = _lfos[i].Next();
  for (int i = 0; i < XTS_SYNTH_ENV_COUNT; i++)
    _output.envelopes[i] = _envs[i].Next();
  return Output();
}

CvDSP::
CvDSP(CvModel const* model, float velo, float bpm, float rate):
_output(), _lfos(), _envs()
{
  _output.velocity = velo;
  for (int i = 0; i < XTS_SYNTH_LFO_COUNT; i++)
    _lfos[i] = LfoDSP(&model->lfos[i], bpm, rate);
  for (int i = 0; i < XTS_SYNTH_ENV_COUNT; i++)
    _envs[i] = EnvDSP(&model->envs[i], bpm, rate);
}

} // namespace Xts