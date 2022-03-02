#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#include <string>
#include <vector>
#include <algorithm>
#include <cassert>

#define MIN_HOLD_MS 1.0f
#define MAX_HOLD_MS 3000.0f

namespace Xts {

struct CycledPlotState
{
  int cycles;
  float frequency;
  PlotFlags flags;
  PlotOutput* output;
  PlotInput const* input;
};

class PlotDSP
{
  static constexpr wchar_t UnicodePi = L'\u03C0';

  static std::vector<VSplit> BiVSPlits;
  static std::vector<VSplit> UniVSPlits;
  static std::vector<VSplit> StereoVSPlits;
  static std::wstring FormatEnv(EnvelopeStage stage);
  static std::vector<VSplit> MakeBiVSplits(float max);

public:
  template <class Factory, class Next>
  static void RenderCycled(CycledPlotState* state, Factory factory, Next next);

  template <
    class Factory, class Next, class Left, class Right, 
    class EnvOutput, class Release, class End>
  static void RenderStaged(
    int hold, PlotFlags flags,
    EnvModel const& envModel, PlotInput const& input, PlotOutput& output,
    Factory factory, Next next, Left left, Right right, EnvOutput envOutput, Release release, End end);

  static void Render(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};

template <class Factory, class Next>
void
PlotDSP::RenderCycled(CycledPlotState* state, Factory factory, Next next)
{
  assert((state->flags & PlotStereo) == 0);
  assert((state->flags & PlotNoResample) == 0);

  float max = 1.0f;
  state->output->max = 1.0f;
  state->output->frequency = state->frequency;
  state->output->clip = false;
  state->output->stereo = false;
  state->output->spectrum = (state->flags & PlotSpectrum) != 0;
  state->output->min = (state->flags & PlotBipolar) != 0 ? -1.0f : 0.0f;
  float idealRate = state->output->frequency * state->input->pixels / state->cycles;
  float cappedRate = std::min(state->input->rate, idealRate);
  state->output->rate = state->output->spectrum? state->input->rate : cappedRate;

  auto dsp = factory(state->output->rate);
  float regular = (state->output->rate * state->cycles / state->output->frequency) + 1.0f;
  float fsamples = state->output->spectrum ? state->output->rate : regular;
  int samples = static_cast<int>(std::ceilf(fsamples));
  for (int i = 0; i < samples; i++)
  {
    float sample = next(dsp);
    max = std::max(max, std::fabs(sample));
    state->output->lSamples->push_back(sample);
  }

  state->output->hSplits->emplace_back(samples, L"");
  for (int i = 0; i < state->cycles * 2; i++)
    state->output->hSplits->emplace_back(samples * i / (state->cycles * 2), std::to_wstring(i) + UnicodePi);
  if ((state->flags & PlotAutoRange) == 0)
  {
    assert(max <= 1.0f);
    *(state->output->vSplits) = (state->flags & PlotBipolar) != 0 ? BiVSPlits : UniVSPlits;
    return;
  }
  
  assert((state->flags & PlotBipolar) != 0);
  for (int i = 0; i < samples; i++) (*state->output->lSamples)[i] /= max;
  *state->output->vSplits = MakeBiVSplits(max);
}

template <
  class Factory, class Next, class Left, class Right, 
  class EnvOutput, class Release, class End>
void PlotDSP::RenderStaged(
  int hold, PlotFlags flags,
  EnvModel const& envModel, PlotInput const& input, PlotOutput& output,
  Factory factory, Next next, Left left, Right right, EnvOutput envOutput, Release release, End end)
{
  assert((flags & PlotAutoRange) == 0);

  output.max = 1.0f;
  output.clip = false;
  output.spectrum = (flags & PlotSpectrum) != 0;
  output.stereo = (flags & PlotStereo) != 0;
  output.min = (flags & PlotBipolar) != 0 ? -1.0f : 0.0f;
  bool noResample = (flags & PlotNoResample) != 0;
  float fhold = Param::TimeFramesF(hold, input.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  float releaseSamples = envModel.sync ? Param::StepFramesF(envModel.rStp, input.bpm, input.rate) : Param::TimeFramesF(envModel.r, input.rate, MIN_HOLD_MS, MAX_HOLD_MS);
  output.rate = output.spectrum || noResample ? input.rate : input.rate * input.pixels / (fhold + releaseSamples);
  fhold *= output.rate / input.rate;
  *output.vSplits = output.stereo? StereoVSPlits: (flags & PlotBipolar) != 0 ? BiVSPlits : UniVSPlits;

  int h = 0;
  int i = 0;
  auto state = factory(output.rate);
  while (!end(state))
  {
    if (h++ == static_cast<int>(fhold))
      output.hSplits->emplace_back(i, FormatEnv(release(state).stage));
    next(state);
    float l = left(state);
    output.clip |= Clip(l);
    output.lSamples->push_back(l);
    float r = output.stereo? right(state): 0.0f;
    output.clip |= Clip(r);
    output.rSamples->push_back(r);
    if (i == 0 || envOutput(state).switchedStage)
      output.hSplits->emplace_back(i, FormatEnv(envOutput(state).stage));
    i++;
  }
}

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP