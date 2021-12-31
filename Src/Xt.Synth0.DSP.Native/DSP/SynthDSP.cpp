#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Reset()
{
  for(int u = 0; u < SynthModel::UnitCount; u++)
    _units[u].Reset();
}

float
SynthDSP::Next(SynthModel const& synth, float rate)
{
  float result = 0.0f;
  for (int u = 0; u < SynthModel::UnitCount; u++)
    result += _units[u].Next(synth.global, synth.units[u], rate);
  return result * synth.amp.lvl / 255.0f;
}

} // namespace Xts