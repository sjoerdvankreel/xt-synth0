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

float
SynthDSP::GlobalAmp(SynthModel const& synth, SynthState const& state) const
{
  float envAmp;
  float amp = Level(synth.global.amp);
  auto env = static_cast<AmpEnv>(synth.global.env);
  switch (env)
  {
  case AmpEnv::NoAmpEnv: envAmp = 1.0f; break;
  case AmpEnv::AmpEnv1: envAmp = state.envs[0]; break;
  case AmpEnv::AmpEnv2: envAmp = state.envs[1]; break;
  default: assert(false); break;
  }
  return amp * envAmp;
}

void
SynthDSP::Next(SynthModel const& synth, float rate, SynthOutput& output)
{
  EnvOutput eout;
  UnitOutput uout;
  SynthState state;

  memset(&output, 0, sizeof(output));
  output.end = true;
  for(int e = 0; e < TrackConstants::EnvCount; e++)
  {
    _envs[e].Next(synth.envs[e], rate, eout);
    state.envs[e] = eout.lvl;
    output.end &= eout.stage == EnvStage::End;
  }
 
  float amp = GlobalAmp(synth, state);
  for (int u = 0; u < TrackConstants::UnitCount; u++)
  {
    _units[u].Next(synth.units[u], rate, uout);
    output.l += uout.l * amp;
    output.r += uout.r * amp;
  }
}

} // namespace Xts