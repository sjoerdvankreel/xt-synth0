#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "../Model/SynthModel.hpp"
#include <vector>
#include <cstdint>

namespace Xts {

struct PlotInput
{
  int32_t rate;
  int32_t pixels;
  SynthModel const* synth;
  PlotInput() = default;
};

struct PlotOutput
{
  float freq;
  int32_t rate;
  XtsBool bipolar;
  float* samples;
  int32_t* splits;
  int32_t splitCount;
  int32_t sampleCount;
  PlotOutput() = default;
};

class PlotDSP
{
private:
  EnvDSP _env;
  UnitDSP _unit;
  std::vector<float> _samples;
  std::vector<int32_t> _splits;

public:
  void Render(PlotInput const& input, PlotOutput& output);

private:
  void RenderEnv(PlotInput const& input, int index, PlotOutput& output);
  void RenderUnit(PlotInput const& input, int index, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP