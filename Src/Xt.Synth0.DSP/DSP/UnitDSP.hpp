#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP 
{
  double _phase = 0.0;
  UnitModel const* const _model;
  AudioInput const* const _input;
public:
  UnitDSP(UnitDSP const&) = delete;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  _phase(0.0), _model(model), _input(input) {}
public:
  AudioOutput Next();
  static void Plot(UnitModel const& model, PlotInput const& input, PlotOutput& output);
private:
  float Freq() const;
  float PwPhase() const;
  float Generate(float freq) const;
  float GenerateAdd(float freq) const;
  float GenerateNaive(NaiveType type, float phase) const;
  float GenerateAdd(float freq, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const;
};

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP