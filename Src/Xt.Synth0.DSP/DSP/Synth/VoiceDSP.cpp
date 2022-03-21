#include <DSP/Synth/VoiceDSP.hpp>

namespace Xts {

StagedParams
VoicePlot::Params() const
{
  StagedParams result;
  result.stereo = true;
  result.bipolar = true;
  result.allowSpectrum = true;
  result.allowResample = false;
  return result;
}

void
VoicePlot::Render(VoiceModel const& model, PlotInput const& input, PlotState& state)
{ std::make_unique<VoicePlot>(&model)->DoRender(input, state); }

VoiceDSP::
VoiceDSP(VoiceModel const* model, int oct, UnitNote note, float velocity, float bpm, float rate):
_cv(&model->cv, velocity, bpm, rate),
_amp(&model->amp, velocity),
_audio(&model->audio, oct, note, rate) {}

} // namespace Xts