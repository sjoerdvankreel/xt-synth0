#ifndef XTS_DSP_SYNTH_DELAY_PLOT_HPP
#define XTS_DSP_SYNTH_DELAY_PLOT_HPP

#include <DSP/Synth/SynthDSP.hpp>
#include <DSP/Synth/GlobalPlot.hpp>

namespace Xts {

class DelayPlot: 
public GlobalPlot
{
public:
  DelayPlot(struct SynthModel const* model) : GlobalPlot(model) {}
public:
  float Left() const { return _dsp.Delay().Output().left; }
  float Right() const { return _dsp.Delay().Output().right; }
public:
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotState& state);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_DELAY_PLOT_HPP