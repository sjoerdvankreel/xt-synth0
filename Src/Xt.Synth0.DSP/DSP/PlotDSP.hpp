#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"
#include <algorithm>

namespace Xts {

class PlotDSP
{
public:
  template <class Factory, class Next>
  static void RenderCycled(
    bool spec, bool bipolar, int cycles, float freq,
    PlotInput const& input, PlotOutput& output, 
    Factory factory, Next next);

  template <class Factory, class Next, class EnvOutput, class Release, class End>
  static void RenderStaged(
    bool spec, bool bipolar, bool stereo, int hold,
    EnvModel const& envModel, PlotInput const& input, PlotOutput& output,
    Factory factory, Next next, EnvOutput envOutput, Release release, End end);

  static void Render(SynthModel const& model, PlotInput const& input, PlotOutput& output);
};

template <class Factory, class Next>
void PlotDSP::RenderCycled(
  bool spec, bool bipolar, int cycles, float freq,
  PlotInput const& input, PlotOutput& output,
  Factory factory, Next next)
{
  output.max = 1.0f;
  output.freq = freq;
  output.spec = spec;
  output.stereo = false;
  output.min = bipolar ? -1.0f : 0.0f;
  float idealRate = output.freq * input.pixels / cycles;
  float cappedRate = std::min(input.rate, idealRate);
  output.rate = spec ? input.rate : cappedRate;

  auto state = factory(output.rate);
  float regular = (output.rate * cycles / output.freq) + 1.0f;
  float fsamples = spec ? input.rate : regular;
  int samples = static_cast<int>(std::ceilf(fsamples));
  for (int i = 0; i < samples; i++)
    output.lSamples->push_back(next(state));

  *output.vSplits = bipolar? BiVSPlits: UniVSPlits;
  output.hSplits->emplace_back(samples, L"");
  for (int i = 0; i < cycles * 2; i++)
    output.hSplits->emplace_back(samples * i / (cycles * 2), std::to_wstring(i) + UnicodePi);
}

template <class Factory, class Next, class EnvOutput, class Release, class End>
void PlotDSP::RenderStaged(
  bool spec, bool bipolar, bool stereo, int hold,
  EnvModel const& envModel, PlotInput const& input, PlotOutput& output,
  Factory factory, Next next, EnvOutput envOutput, Release release, End end)
{
  output.max = 1.0f;
  output.spec = spec;
  output.stereo = stereo;
  output.rate = input.rate;
  output.min = bipolar ? -1.0f : 0.0f;
  *output.vSplits = bipolar ? BiVSPlits : UniVSPlits;
  float fhold = TimeF(hold, input.rate);
  float releaseSamples = envModel.sync ? SyncF(input.bpm, input.rate, envModel.rStp) : TimeF(envModel.r, input.rate);
  output.rate = spec ? input.rate : input.rate * input.pixels / (fhold + releaseSamples);
  fhold *= output.rate / input.rate;

  int h = 0;
  int i = 0;
  auto state = factory(output.rate);
  while (!end(state))
  {
    if (h++ == static_cast<int>(fhold))
      output.hSplits->emplace_back(i, FormatEnv(release(state).stage));
    next(state, output);
    if (i == 0 || envOutput(state).staged)
      output.hSplits->emplace_back(i, FormatEnv(envOutput(state).stage));
    i++;
  }
}

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP