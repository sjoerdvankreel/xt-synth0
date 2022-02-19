#include "CVDSP.hpp"

namespace Xts {

void
CVDSP::Release()
{ 
  for (int e = 0; e < EnvCount; e++) 
    _envs[e].Release(); 
}

void
CVDSP::Next()
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

CVDSP::
CVDSP(CVModel const* model, float velo, float bpm, float rate):
_output(), _lfos(), _envs()
{
  _output.velo = velo;
  for (int l = 0; l < LfoCount; l++) 
    _lfos[l] = LfoDSP(&model->lfos[l], bpm, rate);
  for (int e = 0; e < EnvCount; e++) 
    _envs[e] = EnvDSP(&model->envs[e], bpm, rate);
}

} // namespace Xts