#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "SourceDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP: 
public DSPBase<UnitModel, AudioInput, AudioOutput>
{
  double _phase;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  DSPBase(model, input), _phase(0.0) {}
private:
  float PwPhase() const;
  float Generate(float freq) const;
  float GenerateAdd(float freq) const;
  float GenerateNaive(NaiveType type, float phase) const;
  static float Freq(UnitModel const& model, KeyInput const& input);
  float GenerateAdd(float freq, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const;
public:
  void Next(SourceDSP const& source);
  static void Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<UnitDSP, UnitModel>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP