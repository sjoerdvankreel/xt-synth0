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
  StagedPlotState staged;
  staged.env = &envModel;
  staged.flags = PlotStereo | PlotBipolar | PlotNoResample;
  if (spec) staged.flags |= PlotSpectrum;
  staged.hold = hold;
  staged.input = &input;
  staged.output = &output;

  auto next = [](SynthDSP& dsp) { dsp.Next(); };
  auto end = [](SynthDSP const& dsp) { return dsp.End(); };
  auto release = [](SynthDSP& dsp) { return dsp.Release(); };
  auto left = [](SynthDSP const& dsp) { return dsp.Output().left; };
  auto right = [](SynthDSP const& dsp) { return dsp.Output().right; };
  auto envOutput = [](SynthDSP const& dsp) { return dsp._cv.EnvOutput(dsp._amp.Env()); };
  auto factory = [&](float rate) { return SynthDSP(&model, 4, UnitNote::C, 1.0f, input.bpm, rate); };  
  PlotDSP::RenderStaged(&staged, factory, next, left, right, envOutput, release, end);
}

} // namespace Xts