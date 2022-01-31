#ifndef XTS_LFO_DSP_HPP
#define XTS_LFO_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class LfoDSP: 
private DSPBase<LfoModel, SourceInput>
{
  double _phase;
public:
  LfoDSP() = default;
  LfoDSP(LfoModel const* model, SourceInput const* input):
  DSPBase(model, input), _phase(0.0) {}
private:
  float Generate();
  static float Freq(LfoModel const& model, SourceInput const& input);
public:
  float Next();
  static void Plot(LfoModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(StateSourceDSP<LfoDSP, LfoModel>);
static_assert(PlottableDSP<LfoDSP, LfoModel, SourceInput>);

} // namespace Xts
#endif // XTS_LFO_DSP_HPP