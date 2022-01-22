#include "DSP.hpp"
#include "GlobalDSP.hpp"

namespace Xts {

float
GlobalDSP::Amp(SynthState const& state)
{
  float amp = Level(_model->amp);
  float env = state.envs[0] * Level(_model->env1);
  return amp + (1.0f - amp) * env;
}

} // namespace Xts