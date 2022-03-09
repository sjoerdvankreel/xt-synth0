#ifndef XTS_DSP_SYNTH_PLOT_DSP_HPP
#define XTS_DSP_SYNTH_PLOT_DSP_HPP

#include <DSP/Plot.hpp>
#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>
#include <DSP/EnvSample.hpp>

#include <string>
#include <vector>
#include <cassert>
#include <cstdint>
#include <complex>
#include <algorithm>

namespace Xts {

class PlotDSP
{

  static void Render(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};



} // namespace Xts
#endif // XTS_DSP_SYNTH_PLOT_DSP_HPP