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
  float _amp, _pw, _pan, _amt1, _amt2, _roll, _freq, _incr;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  DSPBase(model, input), 
  _phase(0.0), _blepTri(0.0),
  _amp(Level(_model->amp)), 
  _pw(Level(_model->pw) * MaxPw),
  _pan(MixUni1(_model->pan)),
  _amt1(MixBi2(_model->amt1)),
  _amt2(MixBi2(_model->amt2)),
  _roll(MixUni2(_model->addRoll)),
  _freq(Freq(*_model, _input->key)),
  _incr(_freq / input->source.rate) {}
private:
  static float Freq(UnitModel const& model, KeyInput const& input);
  float Mod(SourceDSP const& source, ModSource mod) const;
  float Mod(ModTarget tgt, float val, float mod1, float mod2) const;
  float Generate(float phase, float freq, float mod1, float mod2);
  float GenerateBlep(float phase, float freq, float mod1, float mod2);
  float GenerateAdd(float phase, float freq, float mod1, float mod2) const;
public:
  void Next(SourceDSP const& source);
  AudioOutput Value() const { return _value; }
  static void Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<UnitDSP, UnitModel>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP