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
AmpDSP::Plot(AmpModel const& model, CvModel const& cv, AudioModel const& audio, PlotInput const& input, PlotOutput& output)
{
  int i = 0;
  int h = 0;
  float plotRate = input.spec? input.rate: 5000;
  int hold = TimeI(input.hold, plotRate);  
  int maxSamples = static_cast<int>(input.spec? input.rate: 5 * plotRate);
  output.min = 0.0f;
  output.max = 1.0f;
  output.stereo = false;
  output.rate = plotRate;

  AmpDSP dsp(&model, 1.0f);
  AudioState audioState = { 0 };
  CvDSP cvDSP(&cv, 1.0f, input.bpm, output.rate);
  while (i++ < maxSamples)
  {
    if (h++ == hold) cvDSP.Release();
    if (dsp.End(cvDSP)) break;
    cvDSP.Next();
    dsp.Next(cvDSP.Output(), audioState);
    output.lSamples->push_back(dsp._amp);
  }

  *output.vSplits = UniVSPlits;
  output.hSplits->emplace_back(0, L"");
  output.hSplits->emplace_back(i - 1, L"");
}

} // namespace Xts