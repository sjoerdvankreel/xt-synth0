#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Reset()
{
  for(int u = 0; u < SynthModel::UnitCount; u++)
    _units[u].Reset();
}

float
SynthDSP::Next(SynthModel const& synth, float rate) const
{
  return 0.0f;
}

} // namespace Xts