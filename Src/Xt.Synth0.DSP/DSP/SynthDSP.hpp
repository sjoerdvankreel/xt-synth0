#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP:
private GeneratorDSP<SynthModel>
{
  EnvDSP _envs[EnvCount];
  UnitDSP _units[UnitCount];
public:
  SynthDSP() = default;
  SynthDSP(SynthDSP const&) = delete;
  SynthDSP(SynthModel const* model, AudioInput const* input);
public:
  void Release();
  AudioOutput Next();
  bool End() const { return _envs[0].End(); }
  static void Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(FiniteGenerator<SynthDSP, SynthModel, AudioOutput>);

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP