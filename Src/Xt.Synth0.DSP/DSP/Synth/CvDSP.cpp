#include <DSP/Synth/CvDSP.hpp>

namespace Xts {

EnvSample
CvDSP::ReleaseAll(int env)
{ 
  for (int i = 0; i < XTS_VOICE_ENV_COUNT; i++) _envs[i].Release();
  return _envs[env].Output();
}

CvState const&
CvDSP::Next()
{
  for (int i = 0; i < XTS_VOICE_LFO_COUNT; i++) _output.lfos[i] = _lfos[i].Next();
  for (int i = 0; i < XTS_VOICE_ENV_COUNT; i++) _output.envs[i] = _envs[i].Next();
  return Output();
}

CvDSP::
CvDSP(CvModel const* model, float velocity, float bpm, float rate):
CvDSP()
{
  _output.velocity = velocity;
  for (int i = 0; i < XTS_VOICE_LFO_COUNT; i++) _lfos[i] = LfoDSP(&model->lfos[i], bpm, rate);
  for (int i = 0; i < XTS_VOICE_ENV_COUNT; i++) _envs[i] = EnvDSP(&model->envs[i], bpm, rate);
}

} // namespace Xts