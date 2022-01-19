#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP
{
  EnvDSP _envs[EnvCount];
  UnitDSP _units[UnitCount];
public:
  SynthDSP() = default;
  SynthDSP(SynthDSP const&) = delete;
public:
  bool End() const { return _envs[0].End(); }
  void Init(SynthModel const& model, AudioInput const& input);
  void Release(SynthModel const& model, AudioInput const& input);
  AudioOutput Next(SynthModel const& model, AudioInput const& input);
  void Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(Generator<SynthDSP, SynthModel, AudioOutput>);

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP