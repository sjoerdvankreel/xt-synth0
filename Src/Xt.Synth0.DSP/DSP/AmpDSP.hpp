#ifndef XTS_AMP_DSP_HPP
#define XTS_AMP_DSP_HPP

#include "DSP.hpp"
#include "SourceDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class AmpDSP:
public DSPBase<AmpModel, AudioInput, float>
{
  float _lfoAmt, _lvl;
public:
  AmpDSP() = default;
  AmpDSP(AmpModel const* model, AudioInput const* input) :
  DSPBase(model, input),
  _lfoAmt(Mix(model->lfoAmt)),
  _lvl(Level(model->lvl) * input->key.amp) {}
public:
  void Next(SourceDSP const& source);
  float Value() const { return _value; }
  bool End(SourceDSP const& source) const { return source.Envs()[static_cast<int>(_model->envSrc)].End(); }
  static void Plot(AmpModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(StatePipeDSP<AmpDSP, AmpModel>);
static_assert(FiniteDependentDSP<AmpDSP, AmpModel, AudioInput, float>);

} // namespace Xts
#endif // XTS_AMP_DSP_HPP