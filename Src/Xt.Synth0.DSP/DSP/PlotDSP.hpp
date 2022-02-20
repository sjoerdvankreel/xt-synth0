#ifndef XTS_PLOT_DSP_HPP
#define XTS_PLOT_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"
#include <algorithm>

namespace Xts {

class PlotDSP
{
public:
  static void Render(SynthModel const& model, PlotInput& input, PlotOutput& output);
  template <class Next> 
  static void RenderCycled(
    int cycles, bool bipolar, float freq, 
    PlotInput const& input, PlotOutput& output, Next next);
};

template <class Next> void
PlotDSP::RenderCycled(
  int cycles, bool bipolar, float freq, 
  PlotInput const& input, PlotOutput& output, Next next)
{
  output.max = 1.0f;
  output.freq = freq;
  output.stereo = false;
  output.min = bipolar ? -1.0f : 0.0f;
  float idealRate = output.freq * input.pixels / cycles;
  float cappedRate = std::min(input.rate, idealRate);
  output.rate = input.spec ? input.rate : cappedRate;

  float regular = (output.rate * cycles / output.freq) + 1.0f;
  float fsamples = input.spec ? input.rate : regular;
  int samples = static_cast<int>(std::ceilf(fsamples));
  for (int i = 0; i < samples; i++)
    output.lSamples->push_back(next(output.rate));

  *output.vSplits = BiVSPlits;
  output.hSplits->emplace_back(samples, L"");
  for (int i = 0; i < cycles * 2; i++)
    output.hSplits->emplace_back(samples * i / (cycles * 2), std::to_wstring(i) + UnicodePi);
}

} // namespace Xts
#endif // XTS_PLOT_DSP_HPP