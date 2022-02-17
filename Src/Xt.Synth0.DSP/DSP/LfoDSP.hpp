#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

inline bool LfoIsBipolar(LfoPolarity plty)
{ return plty == LfoPolarity::Bi || plty == LfoPolarity::BiInv; }
inline bool LfoIsInverted(LfoPolarity plty)
{ return plty == LfoPolarity::BiInv || plty == LfoPolarity::UniInv; }

class LfoDSP: 
public DSPBase<LfoModel, SourceInput, float>
{
  double _phase;
  float _incr, _base, _factor;
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, SourceInput const* input):
  DSPBase(model, input), _phase(0.0),
  _incr(Freq(*_model, *_input) / input->rate),
  _base(LfoIsBipolar(_model->plty) ? 0.0f : 0.5f),
  _factor((LfoIsInverted(_model->plty) ? -1.0f : 1.0f) * (1.0f - _base)) {}
private:
  float Generate();
  static float Freq(LfoModel const& model, SourceInput const& input);
public:
  void Next();
  float Value() const { return _value; }
  bool Bipolar() const { return LfoIsBipolar(_model->plty); };
  static void Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(StateSourceDSP<LfoDSP, LfoModel>);

} // namespace Xts
#endif // XTS_LFO_DSP_HPP