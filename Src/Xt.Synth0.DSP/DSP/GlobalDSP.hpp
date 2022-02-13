#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

#include "DSP.hpp"
#include "SourceDSP.hpp"
#include "GlobalDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class GlobalDSP:
public DSPBase<GlobalModel, AudioInput, float>
{
  float _lfoAmt, _amp;
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model, AudioInput const* input) :
  DSPBase(model, input),
  _lfoAmt(MixBi2(model->lfoAmt)),
  _amp(Level(model->amp) * input->key.amp) {}
public:
  void Next(SourceDSP const& source);
  float Value() const { return _value; }
  bool End(SourceDSP const& source) const { return source.Envs()[static_cast<int>(_model->envSrc)].End(); }
  static void Plot(GlobalModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(StatePipeDSP<GlobalDSP, GlobalModel>);
static_assert(FiniteDependentDSP<GlobalDSP, GlobalModel, AudioInput, float>);

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP