#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <DSP/Param.hpp>
#include <DSP/PlotDSP.hpp>

#include <cassert>

namespace Xts {

static ModSource
ToModSource(AmpLfoSource source)
{
  switch (source)
  {
  case AmpLfoSource::LFO1: return ModSource::LFO1;
  case AmpLfoSource::LFO2: return ModSource::LFO2;
  case AmpLfoSource::LFO3: return ModSource::LFO3;
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
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _unitAmount[i] = Param::Level(model->unitAmount[i]);
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++) _filterAmount[i] = Param::Level(model->filterAmount[i]);
}

FloatSample
AmpDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  float lfo = _ampMod.Modulate({ _amp, false }, cv);
  _level = cv.envs[Env()].value * lfo;
  float pan = BipolarToUnipolar1(_panMod.Modulate({_panning, true}, cv));
  FloatSample panned = { (1.0f - pan) * _level, pan * _level };
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _output += audio.units[i] * pan * _unitAmount[i];
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++) _output += audio.filters[i] * pan * _filterAmount[i];  
  return Output().Sanity();
}

void
AmpDSP::Plot(AmpPlotState* state)
{
  StagedPlotState staged;
  staged.flags = PlotNone;
  staged.env = state->env;
  staged.hold = state->hold;
  staged.input = state->input;
  staged.output = state->output;

  auto val = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<AmpDSP>(state)._amp; };
  auto next = [](std::tuple<CvDSP, AmpDSP>& state) { std::get<AmpDSP>(state).Next(std::get<CvDSP>(state).Next(), {}); };
  auto end = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<CvDSP>(state).End(std::get<AmpDSP>(state).Env()); };
  auto release = [](std::tuple<CvDSP, AmpDSP>& state) { return std::get<CvDSP>(state).ReleaseAll(std::get<AmpDSP>(state).Env()); };
  auto factory = [&](float rate) { return std::make_tuple(CvDSP(state->cv, 1.0f, state->input->bpm, rate), AmpDSP(state->model, 1.0f)); };
  auto envOutput = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<CvDSP>(state).EnvOutput(std::get<AmpDSP>(state).Env()); };
  return PlotDSP::RenderStaged(&staged, factory, next, val, val, envOutput, release, end);
}

} // namespace Xts