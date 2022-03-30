#include <DSP/Shared/Param.hpp>
#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <cassert>

namespace Xts {

float
AmpPlot::ReleaseSamples(float bpm, float rate) const 
{ return EnvPlot::ReleaseSamples(_model->voice.cv.envs[XTS_AMP_ENV], bpm, rate); }

void 
AmpPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{ AmpPlot(&model).DoRender(input, state); }

StagedParams 
AmpPlot::Params() const
{
  StagedParams result;
  result.stereo = false;
  result.bipolar = false;
  result.allowResample = true;
  result.allowSpectrum = false;
  return result;
}

void 
AmpPlot::Init(float bpm, float rate)
{
  new(&_ampDsp) AmpDSP(&_model->voice.amp, 1.0f);
  new(&_cvDsp) CvDSP(&_model->voice.cv, 1.0f, bpm, rate);
  new(&_globalLfoDsp) LfoDSP(&_model->global.lfo, bpm, rate);
}

AmpDSP::
AmpDSP(AmpModel const* model, float velocity):
AmpDSP()
{
  _level = 0.0f;
  _model = model;
  _velocity = velocity;
  _output = FloatSample();
  _panMod = VoiceModDSP(&model->panMod);
  _ampMod = VoiceModDSP(&model->ampMod);
}

FloatSample
AmpDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  _ampMod.Next(cv);
  _panMod.Next(cv);
  float amp = Param::Level(_model->amp);
  _level = cv.envs[XTS_AMP_ENV].value * _ampMod.Modulate({ amp, false });
  float panning = Param::Mix(_model->pan);
  float pan = BipolarToUnipolar1(_panMod.Modulate({panning, true}));
  FloatSample panned = { (1.0f - pan) * _level, pan * _level };
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _output += audio.units[i] * panned * Param::Level(_model->unitAmount[i]);
  for (int i = 0; i < XTS_VOICE_FILTER_COUNT; i++) _output += audio.filters[i] * panned * Param::Level(_model->filterAmount[i]);
  return Output().Sanity();
}

} // namespace Xts