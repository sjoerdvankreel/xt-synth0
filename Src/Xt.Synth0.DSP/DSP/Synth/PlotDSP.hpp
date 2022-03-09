#ifndef XTS_DSP_SYNTH_PLOT_DSP_HPP
#define XTS_DSP_SYNTH_PLOT_DSP_HPP

#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>
#include <DSP/Synth/EnvSample.hpp>

#include <string>
#include <vector>
#include <cassert>
#include <cstdint>
#include <complex>
#include <algorithm>

#define XTS_PLOT_MIN_HOLD_MS 1.0f
#define XTS_PLOT_MAX_HOLD_MS 3000.0f

namespace Xts {

typedef int PlotFlags;
inline constexpr PlotFlags PlotNone = 0x0;
inline constexpr PlotFlags PlotStereo = 0x1;
inline constexpr PlotFlags PlotBipolar = 0x2;
inline constexpr PlotFlags PlotSpectrum = 0x4;
inline constexpr PlotFlags PlotAutoRange = 0x8;
inline constexpr PlotFlags PlotNoResample = 0x10;

struct HSplit { int pos; std::wstring marker; };
struct VSplit { float pos; std::wstring marker; };

struct PlotInput
{
  int32_t hold;
  float bpm, rate, pixels;
};

struct PlotOutput
{
  bool clip, spectrum, stereo;
  float frequency, rate, min, max;
  std::vector<float>* lSamples;
  std::vector<float>* rSamples;
  std::vector<HSplit>* hSplits;
  std::vector<VSplit>* vSplits;
  std::vector<std::complex<float>>* fftData;
  std::vector<std::complex<float>>* fftScratch;
};

struct CycledPlotState
{
  int cycles;
  void* context;
  float frequency;
  PlotFlags flags;
  PlotOutput* output;
  PlotInput const* input;
  float (*dspNext)(void* dsp);
  void (*dspDestroy)(void* dsp);
  void* (*dspCreate)(float rate, void* context);
};

struct StagedPlotState
{
  int hold;
  PlotFlags flags;
  PlotOutput* output;
  EnvModel const* env;
  PlotInput const* input;
};

class PlotDSP
{
  static constexpr wchar_t UnicodePi = L'\u03C0';

  static std::vector<VSplit> BiVSPlits;
  static std::vector<VSplit> UniVSPlits;
  static std::vector<VSplit> StereoVSPlits;
  static std::wstring FormatEnv(EnvStage stage);
  static std::vector<VSplit> MakeBiVSplits(float max);

public:
  static void RenderCycled(CycledPlotState* state);

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