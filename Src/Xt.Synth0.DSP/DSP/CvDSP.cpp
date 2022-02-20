#include "CvDSP.hpp"

namespace Xts {

void
CvDSP::Release() 
{ 
  for (int i = 0; i < EnvCount; i++) 
    _envs[i].Release(); 
}

CvState const&
CvDSP::Next()
{
  for (int i = 0; i < LfoCount; i++) 
    _output.lfos[i] = _lfos[i].Next();
  for (int i = 0; i < EnvCount; i++) 
    _output.envs[i] = _envs[i].Next();
  return Output();
}

CvDSP::
CvDSP(CvModel const* model, float velo, float bpm, float rate):
_output(), _lfos(), _envs()
{
  _output.velo = velo;
  for (int i = 0; i < LfoCount; i++) 
    _lfos[i] = LfoDSP(&model->lfos[i], bpm, rate);
  for (int i = 0; i < EnvCount; i++) 
    _envs[i] = EnvDSP(&model->envs[i], bpm, rate);
}

} // namespace Xts