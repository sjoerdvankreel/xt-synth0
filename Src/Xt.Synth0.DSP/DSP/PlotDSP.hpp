#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"
#include <vector>

namespace Xts {

class PlotDSP
{
public:
  PlotDSP() = default;
  PlotDSP(PlotDSP const&) = delete;
public:
  void Render(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP