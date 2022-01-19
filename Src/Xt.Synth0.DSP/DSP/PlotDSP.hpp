#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include "SynthDSP.hpp"
#include "../Model/SynthModel.hpp"
#include <vector>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN PlotInput
{
  int32_t rate;
  int32_t pixels;
  SynthModel const* synth;
  PlotInput() = default;
};

struct XTS_ALIGN PlotOutput0
{
  float freq;
  int32_t rate;
  XtsBool clip;
  XtsBool bipolar;
  int32_t splitCount;
  int32_t sampleCount;
  float* samples;
  int32_t* splits;
  PlotOutput0() = default;
};

class PlotDSP
{
private:
  SynthDSP _dsp;
  std::vector<float> _samples;
  std::vector<int32_t> _splits;

public:
  void Render(PlotInput const& input, PlotOutput0& output);

private:
  void RenderUnit(PlotInput const& input, int index, PlotOutput0& output);
  void RenderGlobal(PlotInput const& input, PlotFit fit, int32_t rate, PlotOutput0& output);
  void RenderEnv(PlotInput const& input, int index, PlotFit fit, int32_t rate, PlotOutput0& output);
};

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP