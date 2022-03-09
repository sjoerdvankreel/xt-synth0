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

struct StagedPlotState
{
  int hold;
  PlotFlags flags;
  PlotOutput* output;
  EnvModel const* env;
  PlotInput const* input;
};



extern std::vector<VSplit> BiVSPlits;
extern std::vector<VSplit> UniVSPlits;
extern std::vector<VSplit> StereoVSPlits;
extern std::wstring FormatEnv(EnvStage stage);
extern std::vector<VSplit> MakeBiVSplits(float max);

class PlotDSP
{

public:
  template <
    class Factory, class Next, class Left, class Right, 
    class EnvOutput, class Release, class End>
  static void RenderStaged(
    StagedPlotState* state,
    Factory factory, Next next, Left left, Right right, EnvOutput envOutput, Release release, End end);

  static void Render(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};



} // namespace Xts
#endif // XTS_DSP_SYNTH_PLOT_DSP_HPP