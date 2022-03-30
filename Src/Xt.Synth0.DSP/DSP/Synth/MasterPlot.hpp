#ifndef XTS_DSP_SYNTH_MASTER_PLOT_HPP
#define XTS_DSP_SYNTH_MASTER_PLOT_HPP

#include <DSP/Synth/SynthDSP.hpp>
#include <DSP/Synth/GlobalPlot.hpp>

namespace Xts {

class MasterPlot: 
public GlobalPlot
{
public:
  MasterPlot(struct SynthModel const* model) : GlobalPlot(model) {}
public:
  float Left() const { return _dsp.Master().Output().left; }
  float Right() const { return _dsp.Master().Output().right; }
public:
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_MASTER_PLOT_HPP