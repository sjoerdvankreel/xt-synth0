#include "SynthDSP.hpp"
#include "DSP.hpp"

namespace Xts {

void
SynthDSP::Reset()
{
  for(int u = 0; u < TrackConstants::UnitCount; u++)
    _units[u].Reset();
}

void
SynthDSP::Next(SynthModel const& synth, float rate, float* l, float* r)
{
  float ul;
  float ur;
  *l = 0.0f;
  *r = 0.0f;
  bool cycled = false;
  float amp = Level(synth.global.amp);
  for (int u = 0; u < TrackConstants::UnitCount; u++)
  {
    _units[u].Next(synth.units[u], rate, false, &ul, &ur, &cycled);
    *l += ul * amp;
    *r += ur * amp;
  }
}

} // namespace Xts