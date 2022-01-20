#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP: 
private GeneratorDSP<UnitModel>
{
  double _phase;
public:
  UnitDSP() = default;
  UnitDSP(UnitDSP const&) = delete;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  GeneratorDSP(model, input), _phase(0.0) {}
public:
  AudioOutput Next();
  static void Plot(UnitModel const& model, PlotInput const& input, PlotOutput& output);
private:
  float PwPhase() const;
  float Generate(float freq) const;
  float GenerateAdd(float freq) const;
  float GenerateNaive(NaiveType type, float phase) const;
  static float Freq(UnitModel const& model, AudioInput const& input);
  float GenerateAdd(float freq, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const;
};
static_assert(Generator<UnitDSP, UnitModel, AudioOutput>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP