#ifndef XTS_GLOBAL_DSP_HPP
#define XTS_GLOBAL_DSP_HPP

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
  bool End() const;
  void Next(SynthState const& state);
  static void Plot(GlobalModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(StatePipeDSP<GlobalDSP, GlobalModel>);

} // namespace Xts
#endif // XTS_GLOBAL_DSP_HPP