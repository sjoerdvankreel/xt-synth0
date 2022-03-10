#include <DSP/Synth/SynthDSP.hpp>

namespace Xts {

StagedParams
SynthPlot::Params() const
{
  StagedParams result;
  result.stereo = true;
  result.bipolar = true;
  result.allowSpectrum = true;
  result.allowResample = false;
  return result;
}

void
SynthPlot::Render(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  auto plot = std::make_unique<SynthPlot>(&model);
  plot->RenderCore(input, model.plot.hold, output);
}

SynthDSP::
SynthDSP(SynthModel const* model, int oct, UnitNote note, float velocity, float bpm, float rate):
_cv(& model->cv, velocity, bpm, rate),
_amp(&model->amp, velocity),
_audio(&model->audio, oct, note, rate) {}

} // namespace Xts