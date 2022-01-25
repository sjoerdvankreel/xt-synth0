#include "DSP.hpp"
#include "GlobalDSP.hpp"

namespace Xts {

float
GlobalDSP::Amp(SynthState const& state)
{
  float lfoAmt = Level(_model->ampLfoAmt);
  float lfoVal = state.lfos[static_cast<int>(_model->ampLfo)];
  float lfo = (1.0f - lfoAmt) + lfoAmt * lfoVal;
  float amp = Level(_model->amp);
  float envAmt = Level(_model->ampEnvAmt);
  float env = state.envs[static_cast<int>(_model->ampEnv)];
  return (amp * lfo) + (1.0f - amp) * env * envAmt * lfo;
}

} // namespace Xts