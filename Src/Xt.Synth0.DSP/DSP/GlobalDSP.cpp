#include "DSP.hpp"
#include "GlobalDSP.hpp"

namespace Xts {

void
GlobalDSP::Next(SourceDSP const& source)
{
  float amp = Level(_model->amp);
  float lfoAmt = Level(_model->ampLfoAmt);
  float lfoVal = source.Lfos()[static_cast<int>(_model->ampLfo)].Value();
  float lfo = (1.0f - lfoAmt) + lfoAmt * lfoVal;
  float envAmt = Level(_model->ampEnvAmt);
  float envVal = source.Envs()[static_cast<int>(_model->ampEnv)].Value();
  float env = envAmt * envVal;
  _value = (amp * lfo) + (1.0f - amp) * env * lfo;
}

void
GlobalDSP::Plot(GlobalModel const& model, PlotInput const& input, PlotOutput& output)
{
}

} // namespace Xts