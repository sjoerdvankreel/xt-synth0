#include "DSP.hpp"
#include "GlobalDSP.hpp"

namespace Xts {

float
GlobalDSP::Amp(SynthState const& state)
{
  float amp = Level(_model->amp);
  int src = static_cast<int>(_model->ampEnv);
  float env = state.envs[src] * Level(_model->ampEnvAmt);
  return amp + (1.0f - amp) * env;
}

} // namespace Xts