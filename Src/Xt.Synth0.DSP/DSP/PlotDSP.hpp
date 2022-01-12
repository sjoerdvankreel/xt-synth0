#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include "EnvDSP.hpp"
#include "UnitDSP.hpp"
#include "../Model/SynthModel.hpp"
#include <vector>
#include <cstdint>

namespace Xts {

class PlotDSP
{
private:
  EnvDSP _env;
  UnitDSP _unit;
  std::vector<float> _samples;
  std::vector<int32_t> _splits;

  void RenderEnv(
    EnvModel const& env, int32_t pixels, PlotFit fit, int32_t* rate);
  void RenderUnit(
    UnitModel const& unit, int32_t pixels, PlotFit fit, int32_t* rate, float* frequency);
public:
  void Render(
    SynthModel const& synth, int32_t pixels, int32_t* rate, float* frequency,
    float** samples, int32_t* sampleCount, int32_t** splits, int32_t* splitCount);
};

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP