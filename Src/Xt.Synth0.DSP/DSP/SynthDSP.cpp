#include "SynthDSP.hpp"
#include "DSP.hpp"
#include <cassert>
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
  memset(&output, 0, sizeof(output));
  output.end = true;
  int bpm = synth.global.bpm;
  for(int e = 0; e < TrackConstants::EnvCount; e++)
  {
    _envs[e].Next(synth.envs[e], rate, bpm, output.envs[e]);
    output.end &= output.envs[e].stage == EnvStage::End;
  }
 
  float amp = GlobalAmp(synth, output);
  for (int u = 0; u < TrackConstants::UnitCount; u++)
  {
    _units[u].Next(synth.units[u], rate, output.units[u]);
    output.l += output.units[u].l * amp;
    output.r += output.units[u].r * amp;
  }
}

float
SynthDSP::GlobalAmp(SynthModel const& synth, SynthOutput const& output) const
{
  float envAmp = 0.0f;
  float amp = Level(synth.global.amp);
  auto env = static_cast<AmpEnv>(synth.global.env);
  switch (env)
  {
  case AmpEnv::NoAmpEnv: envAmp = 1.0f; break;
  case AmpEnv::AmpEnv1: envAmp = output.envs[0].lvl; break;
  case AmpEnv::AmpEnv2: envAmp = output.envs[1].lvl; break;
  default: assert(false); break;
  }
  return amp * envAmp;
}

} // namespace Xts