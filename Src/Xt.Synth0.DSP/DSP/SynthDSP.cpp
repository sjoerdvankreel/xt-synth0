#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Reset()
{
  for(int u = 0; u < TrackConstants::UnitCount; u++)
    _units[u].Reset();
}

float
SynthDSP::Next(SynthModel const& synth, float rate)
{
  bool reset = false;
  float result = 0.0f;
  for (int u = 0; u < TrackConstants::UnitCount; u++)
    result += _units[u].Next(synth.units[u], rate, &reset);
  return result * synth.global.amp / 255.0f;
}

} // namespace Xts