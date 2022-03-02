#include "AmpDSP.hpp"
#include "PlotDSP.hpp"
#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>

namespace Xts {

AmpDSP::
AmpDSP(AmpModel const* model, float velo):
_amp(0.0f), 
_output(), 
_model(model), 
_units(), _flts(),
_pan(Param::Mix(model->pan)),
_lvlAmt(Param::Mix(model->lvlAmt)),
_panAmt(Param::Mix(model->panAmt)),
_lvl(Param::Level(model->lvl) * velo)
{
  for(int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++)
    _units[i] = Param::Level(model->units[i]);
  for(int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++)
    _flts[i] = Param::Level(model->flts[i]);
}

FloatSample
AmpDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();    

  CvSample lvlLfo = cv.lfos[static_cast<int>(_model->lvlSrc)];
  float lvl = Modulate(_lvl, false, _lvlAmt, lvlLfo);
  int envSrc = static_cast<int>(_model->envSrc);
  _amp = cv.envelopes[static_cast<int>(_model->envSrc)].value * lvl;
  
  CvSample mod = ModulationInput(cv, _model->panSrc);
  float panMix = BipolarToUnipolar1(Modulate(_pan, true, _panAmt, mod));
  FloatSample pan = { (1.0f - panMix) * _amp, panMix * _amp };

  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++)
    _output += audio.units[i] * pan * _units[i];
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++)
    _output += audio.filters[i] * pan * _flts[i];  

  assert(!std::isnan(_output.left));
  assert(!std::isnan(_output.right));
  return Output();
}

void
AmpDSP::Plot(AmpModel const& model, EnvModel const& envModel, CvModel const& cvModel, int hold, PlotInput const& input, PlotOutput& output)
{
  auto val = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<AmpDSP>(state)._amp; };
  auto next = [](std::tuple<CvDSP, AmpDSP>& state) { std::get<AmpDSP>(state).Next(std::get<CvDSP>(state).Next(), {}); };
  auto factory = [&](float rate) { return std::make_tuple(CvDSP(&cvModel, 1.0f, input.bpm, rate), AmpDSP(&model, 1.0f)); };
  auto end = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<CvDSP>(state).End(std::get<AmpDSP>(state).Env()); };
  auto release = [](std::tuple<CvDSP, AmpDSP>& state) { return std::get<CvDSP>(state).ReleaseAll(std::get<AmpDSP>(state).Env()); };
  auto envOutput = [](std::tuple<CvDSP, AmpDSP> const& state) { return std::get<CvDSP>(state).EnvOutput(std::get<AmpDSP>(state).Env()); };
  return PlotDSP::RenderStaged(hold, 0, envModel, input, output, factory, next, val, val, envOutput, release, end);
}

} // namespace Xts