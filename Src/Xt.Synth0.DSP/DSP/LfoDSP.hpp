#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class LfoDSP
{
  double _phase;
  LfoModel const* _model;
  float _bpm, _rate, _value;
  float _incr, _base, _factor;
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, float bpm, float rate) :
  _phase(0.0), _model(model), _bpm(bpm), _rate(rate),
  _value(0.0f), _incr(Freq(*_model, bpm, rate) / rate),
  _base(IsBipolar(_model->plty) ? 0.0f : 0.5f),
  _factor((IsInverted(_model->plty) ? -1.0f : 1.0f) * (1.0f - _base)) {}
public:
  void Next();
  float Value() const { return _value; }
  bool Bipolar() const { return IsBipolar(_model->plty); };
  static void Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output);
private:
  float Generate() const;
  static float Freq(LfoModel const& model, float bpm, float rate);
  static bool IsBipolar(LfoPolarity plty) { return plty == LfoPolarity::Bi || plty == LfoPolarity::BiInv; }
  static bool IsInverted(LfoPolarity plty) { return plty == LfoPolarity::BiInv || plty == LfoPolarity::UniInv; }
};

} // namespace Xts
#endif // XTS_LFO_DSP_HPP