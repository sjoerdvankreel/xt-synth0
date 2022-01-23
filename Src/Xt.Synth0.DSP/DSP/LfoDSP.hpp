#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class LfoDSP: 
private DSPBase<LfoModel>
{
  double _phase;
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, SynthInput const* input):
  DSPBase(model, input), _phase(0.0) {}
public:
  float Next();
  static void Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(PlottableDSP<LfoDSP, LfoModel>);
static_assert(StateSourceDSP<LfoDSP, LfoModel>);

} // namespace Xts
#endif // XTS_LFO_DSP_HPP