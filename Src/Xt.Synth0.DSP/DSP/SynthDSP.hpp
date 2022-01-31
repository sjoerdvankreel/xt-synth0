#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "LfoDSP.hpp"
#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "GlobalDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP:
private DSPBase<SynthModel, AudioInput>
{
  GlobalDSP _global;
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvCount];
  UnitDSP _units[UnitCount];
public:
  SynthDSP() = default;
  SynthDSP(SynthModel const* model, AudioInput const* input);
public:
  AudioOutput Next();
  void Release() { Release(_envs); }
  bool End() const { return End(_envs); }
public:
  void Release(EnvDSP* envs);
  AudioOutput Next(SynthState const& state);
  bool End(EnvDSP const* envs) const { return _global.End(envs); }
  static void Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<SynthDSP, SynthModel>);
static_assert(ReleaseableDSP<SynthDSP, SynthModel, AudioInput>);

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP