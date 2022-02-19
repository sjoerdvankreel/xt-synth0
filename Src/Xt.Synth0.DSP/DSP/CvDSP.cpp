#include "CvDSP.hpp"

namespace Xts {

void
CvDSP::Release()
{ 
  for (int e = 0; e < EnvCount; e++) 
    _envs[e].Release(); 
}

void
CvDSP::Next()
{
  for (int l = 0; l < LfoCount; l++)
  {
    _lfos[l].Next();
    _output.lfos[l] = _lfos[l].Output();
  }
  for (int e = 0; e < EnvCount; e++)
  {
    _envs[e].Next();
    _output.envs[e] = _envs[e].Output();
  }
}

CvDSP::
CvDSP(CvModel const* model, float velo, float bpm, float rate):
_output(), _lfos(), _envs()
{
  _output.velo = velo;
  for (int l = 0; l < LfoCount; l++) 
    _lfos[l] = LfoDSP(&model->lfos[l], bpm, rate);
  for (int e = 0; e < EnvCount; e++) 
    _envs[e] = EnvDSP(&model->envs[e], bpm, rate);
}

} // namespace Xts