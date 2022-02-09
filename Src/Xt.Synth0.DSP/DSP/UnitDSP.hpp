#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "DSP.hpp"
#include "SourceDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class UnitDSP: 
public DSPBase<UnitModel, AudioInput, AudioOutput>
{
  double _phase, _blepTri;
  float _amt1, _amt2, _amp, _pan;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  DSPBase(model, input), 
  _phase(0.0), _blepTri(0.0),
  _amt1(LevelInc(_model->amt1)), 
  _amt2(LevelInc(_model->amt2)), 
  _amp(LevelInc(_model->amp)), 
  _pan(Mix01Inclusive(_model->pan)) {}
private:
  float PwPhase() const;
  float Generate(float freq);
  float GenerateBlep(float freq);
  float GenerateAdd(float freq) const;
  float Amp(float mod1, float mod2) const;
  float Pan(float mod1, float mod2) const;
  float Mod(SourceDSP const& source, ModSource mod) const;
  static float Freq(UnitModel const& model, KeyInput const& input);
public:
  void Next(SourceDSP const& source);
  AudioOutput Value() const { return _value; }
  static void Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<UnitDSP, UnitModel>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP