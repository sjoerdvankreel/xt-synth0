#ifndef XTS_DSP_SYNTH_PLOT_DSP_HPP
#define XTS_DSP_SYNTH_PLOT_DSP_HPP

namespace Xts {

class PlotDSP
{
public:
  static void Render(struct SynthModel const& model, struct PlotInput const& input, struct PlotOutput& output);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_PLOT_DSP_HPP