#include "AmpDSP.hpp"

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

  return Output();
}

void
AmpDSP::Plot(AmpModel const& model, EnvModel const& envModel, CvModel const& cvModel, AudioModel const& audio, PlotInput const& input, PlotOutput& output)
{
  output.min = 0.0f;
  output.max = 1.0f;
  output.stereo = false;
  output.rate = input.rate;
  *output.vSplits = UniVSPlits;
  float hold = TimeF(input.hold, input.rate);
  float release = envModel.sync ? SyncF(input.bpm, input.rate, envModel.rStp) : TimeF(envModel.r, input.rate);
  output.rate = input.spec ? input.rate : input.rate * input.pixels / (hold + release);
  hold *= output.rate / input.rate;

  int h = 0;
  int i = 0;
  AmpDSP dsp(&model, 1.0f);
  AudioState audioState = { 0 };
  CvDSP cvDSP(&cvModel, 1.0f, input.bpm, output.rate);
  while (!cvDSP.End(dsp.Env()))
  {
    if (h++ == static_cast<int>(hold))
      output.hSplits->emplace_back(i, FormatEnv(cvDSP.ReleaseAll(dsp.Env()).stage));
    dsp.Next(cvDSP.Next(), audioState);
    output.lSamples->push_back(dsp._amp);
    if (i == 0 || cvDSP.EnvOutput(dsp.Env()).staged)
      output.hSplits->emplace_back(i, FormatEnv(cvDSP.EnvOutput(dsp.Env()).stage));
    i++;
  }
  output.lSamples->push_back(0.0f);
}

} // namespace Xts