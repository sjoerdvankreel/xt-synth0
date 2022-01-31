#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

#include "../DSP/EnvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class GlobalDSP:
private DSPBase<GlobalModel, SourceInput>
{
public:
  GlobalDSP() = default;
  GlobalDSP(GlobalModel const* model, SourceInput const* input):
  DSPBase(model, input) {}
public:
  float Next(SynthState const& state);
  static void Plot(GlobalModel const& model, PlotInput const& input, PlotOutput& output);
  bool End(EnvDSP const* envs) const { return envs[static_cast<int>(_model->ampEnv)].End(envs); }
};
static_assert(StatePipeDSP<GlobalDSP, GlobalModel>);
static_assert(EndableDSP<GlobalDSP, GlobalModel, SourceInput>);

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP