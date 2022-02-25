#include "SynthDSP.hpp"
#include "PlotDSP.hpp"

namespace Xts {

SynthDSP::
SynthDSP(SynthModel const* model, int oct, UnitNote note, float velo, float bpm, float rate):
_cv(&model->cv, velo, bpm, rate),
_amp(&model->amp, velo),
_audio(&model->audio, oct, note, rate) {}

void
SynthDSP::Plot(SynthModel const& model, EnvModel const& envModel, bool spec, int hold, PlotInput const& input, PlotOutput& output)
{
  auto end = [](SynthDSP const& dsp) { return dsp.End(); };
  auto release = [](SynthDSP& dsp) { return dsp.Release(); };
  auto envOutput = [](SynthDSP const& dsp) { return dsp._cv.EnvOutput(dsp._amp.Env()); };
  auto factory = [&](float rate) { return SynthDSP(&model, 4, UnitNote::C, 1.0f, input.bpm, rate); };
  auto next = [](SynthDSP& dsp, PlotOutput& output) 
  { 
    dsp.Next(); 
    output.lSamples->push_back(dsp.Output().l); 
    output.rSamples->push_back(dsp.Output().r); 
  };
  PlotFlags flags = PlotStereo | PlotBipolar | PlotNoResample;
  if (spec) flags |= PlotSpec;
  PlotDSP::RenderStaged(hold, flags, envModel, input, output, factory, next, envOutput, release, end);
  output.vSplits->clear();
  output.vSplits->emplace_back(-0.5f, L"L");
  output.vSplits->emplace_back(0.5f, L"R");
  output.vSplits->emplace_back(0.0f, L"-+");
  output.vSplits->emplace_back(1.0f, L"-1");
  output.vSplits->emplace_back(-1.0f, L"+1");
}

} // namespace Xts