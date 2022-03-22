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
  new(&_globalLfoDsp) LfoDSP(&_model->globalLfo, bpm, rate);
}

static ModSource
ToModSource(AmpLfoSource source)
{
  switch (source)
  {
  case AmpLfoSource::LFO1: return ModSource::LFO1;
  case AmpLfoSource::LFO2: return ModSource::LFO2;
  case AmpLfoSource::GlobalLFO: return ModSource::GlobalLFO;
  default: assert(false); return ModSource::LFO1;
  }
}

AmpDSP::
AmpDSP(AmpModel const* model, float velocity):
AmpDSP()
{
  _level = 0.0f;
  _model = model;
  _output = FloatSample();
  _panning = Param::Mix(model->panning);
  _amp = Param::Level(model->amp) * velocity;
  _panMod = ModDSP(model->panModSource, model->panModAmount);
  _ampMod = ModDSP(ToModSource(model->ampLfoSource), model->ampLfoAmount);
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _unitAmount[i] = Param::Level(model->unitAmount[i]);
  for (int i = 0; i < XTS_VOICE_FILTER_COUNT; i++) _filterAmount[i] = Param::Level(model->filterAmount[i]);
}

FloatSample
AmpDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  _ampMod.Next(cv);
  _panMod.Next(cv);
  _level = cv.envs[XTS_AMP_ENV].value * _ampMod.Modulate({ _amp, false });
  float pan = BipolarToUnipolar1(_panMod.Modulate({_panning, true}));
  FloatSample panned = { (1.0f - pan) * _level, pan * _level };
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _output += audio.units[i] * panned * _unitAmount[i];
  for (int i = 0; i < XTS_VOICE_FILTER_COUNT; i++) _output += audio.filters[i] * panned * _filterAmount[i];
  return Output().Sanity();
}

} // namespace Xts