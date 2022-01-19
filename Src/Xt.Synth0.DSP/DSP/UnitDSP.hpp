#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP 
{
  int _noteOffset = 0;
  double _phase = 0.0;
public:
  UnitDSP() = default;
  UnitDSP(UnitDSP const&) = delete;
public:  
  bool End() const { return false; }
  void Init(UnitModel const& model, AudioInput const& input);
  void Release(UnitModel const& model, AudioInput const& input) {}
  AudioOutput Next(UnitModel const& model, AudioInput const& input);
  void Plot(UnitModel const& model, PlotInput const& input, PlotOutput& output);
private:
  float PwPhase(int pw) const;
  float Freq(UnitModel const& model) const;
  float GenerateNaive(NaiveType type, int pw, float phase) const;
  float Generate(UnitModel const& model, float freq, float rate) const;
  float GenerateAdd(UnitModel const& model, float freq, float rate) const;
  float GenerateAdd(float freq, float rate, float phase, int parts, int step, float logRoll, bool addSub, bool sinCos) const;
};
static_assert(Generator<UnitDSP, UnitModel, AudioOutput>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP