#include "SynthDSP.hpp"

namespace Xts {

SynthDSP::
SynthDSP(SynthModel const* model, int oct, UnitNote note, float velo, float bpm, float rate):
_cv(&model->cv, velo, bpm, rate),
_amp(&model->amp, velo),
_audio(&model->audio, oct, note, rate) {}

void
SynthDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  float plotRate = input.spec ? input.rate : 5000;
  int hold = TimeI(input.hold, plotRate);
  int maxSamples = static_cast<int>(input.spec ? input.rate : 5 * plotRate);
  
  output.max = 1.0f;
  output.min = -1.0f;
  output.rate = plotRate;
  SynthDSP dsp(&model, 4, UnitNote::C, 1.0f, input.bpm, plotRate);
  while (i++ < maxSamples)
  {
    if (h++ == hold) dsp.Release();
    if (dsp.End()) break;
    dsp.Next();
    auto audio = dsp.Output();
    float sample = l ? audio.l : audio.r;
    output.clip |= Clip(sample);
    output.samples->push_back(sample);
  }

  output.hSplits->emplace_back(0, L"");
  output.hSplits->emplace_back(i - 1, L"");
  output.vSplits->emplace_back(0.0f, L"0");
  output.vSplits->emplace_back(1.0f, L"-1");
  output.vSplits->emplace_back(-1.0f, L"1");
}

} // namespace Xts