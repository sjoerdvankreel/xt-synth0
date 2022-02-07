#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "SourceDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP: 
public DSPBase<UnitModel, AudioInput, AudioOutput>
{
  float _last;
  double _phase;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  DSPBase(model, input), _last(0.0f), _phase(0.0) {}
private:
  float PwPhase() const;
  float Generate(float freq) const;
  float GenerateAdd(float freq) const;
  float GenerateNaive(WaveType type, float phase) const;
  float GenerateBlep(WaveType type, float freq, float phase) const;
  static float Freq(UnitModel const& model, KeyInput const& input);
  float GenerateAdd(float freq, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const;
public:
  void Next(SourceDSP const& source);
  AudioOutput Value() const { return _value; }
  static void Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<UnitDSP, UnitModel>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP