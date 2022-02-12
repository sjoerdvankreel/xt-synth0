#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class LfoDSP: 
public DSPBase<LfoModel, SourceInput, float>
{
  double _phase;
  float _base, _factor;
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, SourceInput const* input):
  DSPBase(model, input), _phase(0.0),
  _base(_model->bip ? 0.0f : 0.5f),
  _factor((_model->inv ? -1.0f : 1.0f) * (1.0f - _base)) {}
private:
  float Generate();
  static float Freq(LfoModel const& model, SourceInput const& input);
public:
  void Next();
  float Value() const { return _value; }
  static void Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(StateSourceDSP<LfoDSP, LfoModel>);

} // namespace Xts
#endif // XTS_LFO_DSP_HPP