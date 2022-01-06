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
  float result = 0.0f;
  for (int u = 0; u < TrackConstants::UnitCount; u++)
    result += _units[u].Next(synth.units[u], rate);
  return result * synth.global.lvl / 255.0f;
}

} // namespace Xts