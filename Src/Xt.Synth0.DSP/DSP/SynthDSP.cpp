#include "SynthDSP.hpp"
#include "DSP.hpp"
#include <cstring>

namespace Xts {

void
SynthDSP::Release()
{
  for (int e = 0; e < TrackConstants::EnvCount; e++)
    _envs[e].Release();
}

void
SynthDSP::Init(int oct, UnitNote note)
{
  for(int e = 0; e < TrackConstants::EnvCount; e++)
    _envs[e].Init();
  for(int u = 0; u < TrackConstants::UnitCount; u++)
    _units[u].Init(oct, note);
}

void
SynthDSP::Next(SynthModel const& synth, float rate, SynthOutput& output)
{
  UnitOutput uout;
  memset(&output, 0, sizeof(output));
  float amp = Level(synth.global.amp);
  for (int u = 0; u < TrackConstants::UnitCount; u++)
  {
    _units[u].Next(synth.units[u], rate, uout);
    output.l += uout.l * amp;
    output.r += uout.r * amp;
  }
}

} // namespace Xts