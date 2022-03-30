#include <DSP/Synth/GlobalPlot.hpp>

namespace Xts {

StagedParams
GlobalPlot::Params() const
{
  StagedParams result;
  result.stereo = true;
  result.bipolar = true;
  result.allowSpectrum = true;
  result.allowResample = false;
  return result;
}

} // namespace Xts