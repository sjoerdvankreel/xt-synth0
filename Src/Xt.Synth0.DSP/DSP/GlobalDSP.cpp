#include "DSP.hpp"
#include "GlobalDSP.hpp"

namespace Xts {

float
GlobalDSP::Amp(SynthState const& state)
{
  //float amp = Level(_model->amp);
  //float envAmt = Level(_model->ampEnvAmt);
  float lfoAmt = Level(_model->ampLfoAmt);
  //float env = state.envs[static_cast<int>(_model->ampEnv)];
  float lfoVal = state.lfos[static_cast<int>(_model->ampLfo)];
  return (1.0 - lfoAmt) + lfoAmt * lfoVal;
  
  // return amp * lfo * lfoAmt;
  //return (amp * lfo * lfoAmt) + (1.0f - amp) * env * envAmt * lfo * lfoAmt;
}

} // namespace Xts