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
  auto next = [](SynthDSP& dsp) { dsp.Next(); };
  auto end = [](SynthDSP const& dsp) { return dsp.End(); };
  auto release = [](SynthDSP& dsp) { return dsp.Release(); };
  auto left = [](SynthDSP const& dsp) { return dsp.Output().l; };
  auto right = [](SynthDSP const& dsp) { return dsp.Output().r; };
  auto envOutput = [](SynthDSP const& dsp) { return dsp._cv.EnvOutput(dsp._amp.Env()); };
  auto factory = [&](float rate) { return SynthDSP(&model, 4, UnitNote::C, 1.0f, input.bpm, rate); };
  PlotFlags flags = PlotStereo | PlotBipolar | PlotNoResample;
  if (spec) flags |= PlotSpec;
  PlotDSP::RenderStaged(hold, flags, envModel, input, output, factory, next, left, right, envOutput, release, end);
}

} // namespace Xts