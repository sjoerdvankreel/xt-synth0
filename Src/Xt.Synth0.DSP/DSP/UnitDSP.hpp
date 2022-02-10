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
  static constexpr float MaxPw = 0.975f;
  double _phase, _blepTri;
  float _amt1, _amt2, _amp, _pw, _pan, _roll;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  DSPBase(model, input), 
  _phase(0.0), _blepTri(0.0),
  _amt1(LevelInc(_model->amt1)), 
  _amt2(LevelInc(_model->amt2)), 
  _amp(LevelInc(_model->amp)), 
  _pw(LevelExc(_model->pw) * MaxPw),
  _pan(Mix01Inclusive(_model->pan)),
  _roll(Mix01Inclusive(_model->addRoll)) {}
private:
  float Mod(SourceDSP const& source, ModSource mod) const;
  float Modulate(ModTarget tgt, float val, float mod1, float mod2) const;
  float Generate(float phase, float freq, float mod1, float mod2);
  float GenerateBlep(float phase, float freq, float mod1, float mod2);
  float GenerateAdd(float phase, float freq, float mod1, float mod2) const;
  static float Freq(UnitModel const& model, KeyInput const& input);
public:
  void Next(SourceDSP const& source);
  AudioOutput Value() const { return _value; }
  static void Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<UnitDSP, UnitModel>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP