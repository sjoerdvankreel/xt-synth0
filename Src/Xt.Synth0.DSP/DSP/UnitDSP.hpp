#ifndef XTS_UNIT_DSP_HPP
#define XTS_UNIT_DSP_HPP

#include "DSP.hpp"
#include "SourceDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct ModParams
{
  bool bip1, bip2;
  float mod1, mod2;
public:
  ModParams(ModParams const&) = default;
  ModParams(float mod1, bool bip1, float mod2, bool bip2):
  bip1(bip1), bip2(bip2), mod1(mod1), mod2(mod2) {}
};

class UnitDSP: 
public DSPBase<UnitModel, AudioInput, AudioOutput>
{
  static constexpr float MaxPw = 0.975f;
  double _phase, _blepTri;
  float _amp, _pan, _amt1, _amt2, _velo, _pw, _roll, _freq, _incr;
public:
  UnitDSP() = default;
  UnitDSP(UnitModel const* model, AudioInput const* input):
  DSPBase(model, input), 
  _phase(0.0), _blepTri(0.0),
  _amp(Level(_model->amp)), 
  _pan(Mix(_model->pan)),
  _amt1(Mix(_model->amt1)),
  _amt2(Mix(_model->amt2)),
  _velo(Level(input->key.amp)),
  _pw(Level(_model->pw) * MaxPw),
  _roll(MixUni2(_model->addRoll)),
  _freq(Freq(*_model, _input->key)),
  _incr(_freq / input->source.rate) {}
private:
  static float Freq(UnitModel const& model, KeyInput const& input);
  ModParams Params(SourceDSP const& source);
  bool ModBip(SourceDSP const& source, ModSource mod) const;
  float ModVal(SourceDSP const& source, ModSource mod) const;
  float Mod(ModTarget tgt, float val, bool bip, ModParams const& params) const;
  float Generate(float phase, float freq, ModParams const& params);
  float GenerateBlep(float phase, float freq, ModParams const& params);
  float GenerateAdd(float phase, float freq, ModParams const& params) const;
public:
  void Next(SourceDSP const& source);
  AudioOutput Value() const { return _value; }
  static void Plot(UnitModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(AudioSourceDSP<UnitDSP, UnitModel>);

} // namespace Xts
#endif // XTS_UNIT_DSP_HPP