#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

#include "DSP.hpp"
#include "SourceDSP.hpp"
#include "GlobalDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class GlobalDSP:
public DSPBase<GlobalModel, SourceInput, float>
{
  float _amp, _keyAmt, _lfoAmt, _envAmt;
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model, SourceInput const* input) :
  DSPBase(model, input),
  _amp(Level(model->amp)),
  _keyAmt(Mix(model->keyAmt)),
  _lfoAmt(Mix(model->lfoAmt)),
  _envAmt(Mix(model->envAmt)) {}
public:
  void Next(SourceDSP const& source);
  float Value() const { return _value; }
  bool End(SourceDSP const& source) const { return source.Envs()[static_cast<int>(_model->envSrc)].End(); }
  static void Plot(GlobalModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(StatePipeDSP<GlobalDSP, GlobalModel>);
static_assert(FiniteDependentDSP<GlobalDSP, GlobalModel, SourceInput, float>);

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP