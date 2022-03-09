#include <DSP/Synth/AmpDSP.hpp>
#include <DSP/Synth/CvDSP.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/PlotDSP.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <DSP/Param.hpp>

#include <cassert>

namespace Xts {



  auto val = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<AmpDSP>(state)._level; };
  auto next = [](std::tuple<CvDSP, AmpDSP>& state) { std::get<AmpDSP>(state).Next(std::get<CvDSP>(state).Next(), {}); };
  auto end = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<CvDSP>(state).End(std::get<AmpDSP>(state).Env()); };
  auto release = [](std::tuple<CvDSP, AmpDSP>& state) { return std::get<CvDSP>(state).ReleaseAll(std::get<AmpDSP>(state).Env()); };
  auto factory = [&](float rate) { return std::make_tuple(CvDSP(state->cv, 1.0f, state->input->bpm, rate), AmpDSP(state->model, 1.0f)); };
  auto envOutput = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<CvDSP>(state).EnvOutput(std::get<AmpDSP>(state).Env()); };
  return PlotDSP::RenderStaged(&staged, factory, next, val, val, envOutput, release, end);

class AmpPlot: public StagedPlot
{
  CvDSP _cvDsp;
  AmpDSP _ampDSP;
  CvModel const* _cv;
  AmpModel const* _amp;
public:
  AmpPlot(CvModel const* cv, AmpModel const* amp):
  _cv(cv), _amp(amp) {}

  void Next() { _dsp.Next(); };
  float Right() const { return 0.0f; }
  bool Stereo() const { return false; }
  bool Bipolar() const { return false; }
  bool End() const { return _dsp.End(); }
  bool AllowResample() const { return true; }
  EnvSample Release() { return _dsp.Release(); };
  float Left() const { return _dsp.Output().value; }
  EnvSample EnvOutput() const { return _dsp.Output(); }
  void Init(float bpm, float rate) { _dsp = EnvDSP(_model, bpm, rate); }

  float ReleaseSamples(float bpm, float rate) const
  { return Param::SamplesF(_model->sync, _model->releaseTime, _model->releaseStep, bpm, rate, ENV_MIN_TIME_MS, ENV_MAX_TIME_MS); }
};

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
  float lfo = _ampMod.Modulate({ _amp, false }, _ampMod.Modulator(cv));
  _level = cv.envs[Env()].value * lfo;
  float pan = BipolarToUnipolar1(_panMod.Modulate({_panning, true}, _panMod.Modulator(cv)));
  FloatSample panned = { (1.0f - pan) * _level, pan * _level };
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _output += audio.units[i] * panned * _unitAmount[i];
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++) _output += audio.filters[i] * panned * _filterAmount[i];
  return Output().Sanity();
}

} // namespace Xts