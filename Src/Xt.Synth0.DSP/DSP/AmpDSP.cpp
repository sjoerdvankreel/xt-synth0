#include "AmpDSP.hpp"
#include "PlotDSP.hpp"

namespace Xts {

AmpDSP::
AmpDSP(AmpModel const* model, float velo):
_amp(0.0f), 
_output(), 
_model(model), 
_units(), _flts(),
_pan(Mix(model->pan)),
_lvlAmt(Mix(model->lvlAmt)),
_panAmt(Mix(model->panAmt)),
_lvl(Level(model->lvl) * velo) 
{
  for(int i = 0; i < UnitCount; i++) 
    _units[i] = Level(model->units[i]);
  for(int i = 0; i < FilterCount; i++) 
    _flts[i] = Level(model->flts[i]);
}

AudioOutput
AmpDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();    

  CvOutput lvlLfo = cv.lfos[static_cast<int>(_model->lvlSrc)];
  float lvl = Modulate(_lvl, false, _lvlAmt, lvlLfo);
  int envSrc = static_cast<int>(_model->envSrc);
  _amp = cv.envs[static_cast<int>(_model->envSrc)].val * lvl;
  
  CvOutput mod = ModulationInput(cv, _model->panSrc);
  float panMix = BiToUni1(Modulate(_pan, true, _panAmt, mod));
  AudioOutput pan = { (1.0f - panMix) * _amp, panMix * _amp };

  for (int i = 0; i < UnitCount; i++) 
    _output += audio.units[i] * pan * _units[i];
  for (int i = 0; i < FilterCount; i++) 
    _output += audio.filts[i] * pan * _flts[i];  

  assert(!std::isnan(_output.l));
  assert(!std::isnan(_output.r));
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