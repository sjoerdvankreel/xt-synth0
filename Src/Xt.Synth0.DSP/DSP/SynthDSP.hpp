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
public DSPBase<SynthModel, AudioInput, AudioOutput>
{
  GlobalDSP _global;
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvCount];
  UnitDSP _units[UnitCount];
public:
  SynthDSP() = default;
  SynthDSP(SynthModel const* model, AudioInput const* input);
public:
  void Release();
  AudioOutput Next();
  void Next(SynthState const& state);
  bool End() const { return _envs[static_cast<int>(_model->global.ampEnv)].End(); }
  static void Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<SynthDSP, SynthModel>);
static_assert(FiniteDSP<SynthDSP, SynthModel, AudioInput, AudioOutput>);

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP