#ifndef XTS_SYNTH_DSP_HPP
#define XTS_SYNTH_DSP_HPP

#include "UnitDSP.hpp"
#include "SourceDSP.hpp"
#include "GlobalDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SynthDSP:
public DSPBase<SynthModel, AudioInput, AudioOutput>
{
  SourceDSP _source;
  GlobalDSP _global;
  UnitDSP _units[UnitCount];
public:
  SynthDSP() = default;
  SynthDSP(SynthModel const* model, AudioInput const* input);
public:
  void Next(SourceDSP const& source);
  void Release() { _source.Release(); }
  bool End() const { return End(_source); }
  AudioOutput Value() const { return _value; }
  void Next() { _source.Next(); return Next(_source); };
  bool End(SourceDSP const& source) const { return _global.End(source); }
  static void Plot(SynthModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<SynthDSP, SynthModel>);
static_assert(ReleaseableDSP<SynthDSP, SynthModel, AudioInput, AudioOutput>);
static_assert(FiniteSourceDSP<SynthDSP, SynthModel, AudioInput, AudioOutput>);
static_assert(FiniteDependentDSP<SynthDSP, SynthModel, AudioInput, AudioOutput>);

} // namespace Xts
#endif // XTS_SYNTH_DSP_HPP