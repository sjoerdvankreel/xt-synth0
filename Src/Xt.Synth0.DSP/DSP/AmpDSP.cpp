#include "AmpDSP.hpp"

namespace Xts {

AmpDSP::
AmpDSP(AmpModel const* model, float velo):
_amp(0.0f), _output(), _model(model),
_flt1(Level(model->flt1)),
_flt2(Level(model->flt2)),
_flt3(Level(model->flt3)),
_unit1(Level(model->unit1)),
_unit2(Level(model->unit2)),
_unit3(Level(model->unit3)),
_pan(Mix(model->pan)),
_lvlAmt(Mix(model->lvlAmt)),
_panAmt(Mix(model->panAmt)),
_lvl(Level(model->lvl) * velo) {}

void
AmpDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.l = 0.0f;
  _output.r = 0.0f;

  CvOutput lvlLfo = cv.lfos[static_cast<int>(_model->lvlSrc)];
  float lvl = Modulate(_lvl, false, _lvlAmt, lvlLfo);
  int envSrc = static_cast<int>(_model->envSrc);
  _amp = cv.envs[static_cast<int>(_model->envSrc)] * lvl;

  CvOutput mod = ModulationInput(cv, _model->panSrc);
  float pan = BiToUni1(Modulate(_pan, true, _panAmt, mod));
  float l = (1.0f - pan) * _amp;
  float r = pan * _amp;

  _output.l += _flt1 * l * audio.filts[0].l;
  _output.r += _flt1 * r * audio.filts[0].r;
  _output.l += _flt2 * l * audio.filts[1].l;
  _output.r += _flt2 * r * audio.filts[1].r;
  _output.l += _flt3 * l * audio.filts[2].l;
  _output.r += _flt3 * r * audio.filts[2].r;
  _output.l += _unit1 * l * audio.units[0].l;
  _output.r += _unit1 * r * audio.units[0].r;
  _output.l += _unit2 * l * audio.units[1].l;
  _output.r += _unit2 * r * audio.units[1].r;
  _output.l += _unit3 * l * audio.units[2].l;
  _output.r += _unit3 * r * audio.units[2].r;
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

  output.hSplits->emplace_back(0, L"");
  output.hSplits->emplace_back(i - 1, L"");
  output.vSplits->emplace_back(0.0f, L"1");
  output.vSplits->emplace_back(1.0f, L"0");
  output.vSplits->emplace_back(0.5f, L"\u00BD");
  output.vSplits->emplace_back(0.25f, L"\u00BE");
  output.vSplits->emplace_back(0.75f, L"\u00BC");
}

} // namespace Xts