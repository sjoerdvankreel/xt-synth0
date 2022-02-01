#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

#include "SourceDSP.hpp"
#include "GlobalDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class GlobalDSP:
public DSPBase<GlobalModel, SourceInput, float>
{
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model, SourceInput const* input):
  DSPBase(model, input) {}
public:
  void Next(SourceDSP const& source);
  float Value() const { return _value; }
  bool End(SourceDSP const& source) const { return source.Envs()[static_cast<int>(_model->ampEnv)].End(); }
  static void Plot(GlobalModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output);
};
static_assert(StatePipeDSP<GlobalDSP, GlobalModel>);
static_assert(FiniteDependentDSP<GlobalDSP, GlobalModel, SourceInput, float>);

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP