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

#define XTS_PLOT_MIN_HOLD_MS 1.0f
#define XTS_PLOT_MAX_HOLD_MS 3000.0f

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

template <
  class Factory, class Next, class Left, class Right, 
  class EnvOutput, class Release, class End>
void PlotDSP::RenderStaged(
  StagedPlotState* state,
  Factory factory, Next next, Left left, Right right, EnvOutput envOutput, Release release, End end)
{
  assert((state->flags & PlotAutoRange) == 0);

  state->output->max = 1.0f;
  state->output->clip = false;
  state->output->spectrum = (state->flags & PlotSpectrum) != 0;
  state->output->stereo = (state->flags & PlotStereo) != 0;
  state->output->min = (state->flags & PlotBipolar) != 0 ? -1.0f : 0.0f;
  bool noResample = (state->flags & PlotNoResample) != 0;
  float fhold = Param::TimeSamplesF(state->hold, state->input->rate, XTS_PLOT_MIN_HOLD_MS, XTS_PLOT_MAX_HOLD_MS);
  float releaseSamples = Param::SamplesF(state->env->sync, state->env->releaseTime, state->env->releaseStep, state->input->bpm, state->input->rate, XTS_PLOT_MIN_HOLD_MS, XTS_PLOT_MAX_HOLD_MS);
  state->output->rate = state->output->spectrum || noResample ? state->input->rate : state->input->rate * state->input->pixels / (fhold + releaseSamples);
  fhold *= state->output->rate / state->input->rate;
  *(state->output->vSplits) = state->output->stereo? StereoVSPlits: (state->flags & PlotBipolar) != 0 ? BiVSPlits : UniVSPlits;

  int h = 0;
  int i = 0;
  auto dsp = factory(state->output->rate);
  while ((!state->output->spectrum && !end(dsp)) || (state->output->spectrum && i < static_cast<int>(state->output->rate)))
  {
    if (!state->output->spectrum && h++ == static_cast<int>(fhold))
      state->output->hSplits->emplace_back(i, FormatEnv(release(dsp).stage));
    next(dsp);
    float l = left(dsp);
    state->output->clip |= Clip(l);
    state->output->lSamples->push_back(l);
    float r = state->output->stereo? right(dsp): 0.0f;
    state->output->clip |= Clip(r);
    state->output->rSamples->push_back(r);
    if (i == 0 || envOutput(dsp).switchedStage)
      state->output->hSplits->emplace_back(i, FormatEnv(envOutput(dsp).stage));
    i++;
  }
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_PLOT_DSP_HPP