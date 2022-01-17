#include "SynthDSP.hpp"
#include "DSP.hpp"
#include <cassert>
#include <cstring>

namespace Xts {

void
SynthDSP::Release(SynthModel const& synth)
{
  for (int e = 0; e < TrackConstants::EnvCount; e++)
    _envs[e].Release(synth.envs[e]);
}

void
SynthDSP::Init(SynthModel const& synth, int oct, UnitNote note)
{
  for(int e = 0; e < TrackConstants::EnvCount; e++)
    _envs[e].Init(synth.envs[e]);
  for(int u = 0; u < TrackConstants::UnitCount; u++)
    _units[u].Init(oct, note);
}

void
SynthDSP::Next(SynthModel const& synth, float rate, SynthOutput& output)
{
  memset(&output, 0, sizeof(output));
  output.end = true;
  int bpm = synth.global.bpm;
  for(int e = 0; e < TrackConstants::EnvCount; e++)
  {
    _envs[e].Next(synth.envs[e], rate, bpm, output.envs[e]);
    output.end &= output.envs[e].stage == EnvStage::End;
  }
 
  float amp = output.envs[0].lvl * Level(synth.global.env1);
  for (int u = 0; u < TrackConstants::UnitCount; u++)
  {
    _units[u].Next(synth.units[u], rate, output.units[u]);
    output.l += output.units[u].l * amp;
    output.r += output.units[u].r * amp;
  }
}

} // namespace Xts