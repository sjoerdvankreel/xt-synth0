#include <DSP/Synth/SynthDSP.hpp>

namespace Xts {

StagedParams
SynthPlot::Params() const
{
  StagedParams result;
  result.stereo = true;
  result.bipolar = true;
  result.allowResample = false;
  return result;
}

SynthDSP::
SynthDSP(SynthModel const* model, int oct, UnitNote note, float velo, float bpm, float rate):
_cv(&model->cv, velo, bpm, rate),
_amp(&model->amp, velo),
_audio(&model->audio, oct, note, rate) {}

void
SynthPlot::Render(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  auto plot = std::make_unique<SynthPlot>(&model);
  plot->RenderCore(input, model.plot.hold, output);
}

} // namespace Xts