#include "AmpDSP.hpp"

namespace Xts {

void
AmpDSP::Next(SourceDSP const& source)
{
  int lfoSrc = static_cast<int>(_model->lfoSrc);
  auto const& lfo = source.Lfos()[lfoSrc];
  float lvl = Mod(_lvl, false, lfo.Value(), lfo.Bipolar(), _lfoAmt);
  int envSrc = static_cast<int>(_model->envSrc);
  _value = source.Envs()[envSrc].Value() * lvl;
}

void
AmpDSP::Plot(AmpModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output)
{
  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  float plotRate = input.spec? input.rate: 5000;
  int hold = TimeI(input.hold, plotRate);  
  int maxSamples = static_cast<int>(input.spec? input.rate: 5 * plotRate);
  output.min = 0.0f;
  output.max = 1.0f;
  output.rate = plotRate;
  KeyInput keyInput(4, UnitNote::C, 1.0f);
  SourceInput sourceInput(plotRate, input.bpm);
  AudioInput audioInput(sourceInput, keyInput);
  AmpDSP dsp(&model, &audioInput);
  SourceDSP sourceDsp(&source, &sourceInput);
  while (i++ < maxSamples)
  {
    if (h++ == hold) sourceDsp.Release();
    if (dsp.End(sourceDsp)) break;
    sourceDsp.Next();
    dsp.Next(sourceDsp);
    output.samples->push_back(dsp.Value());
  }

  output.hSplits->emplace_back(HSplit(0, L""));
  output.hSplits->emplace_back(HSplit(i - 1, L""));
  output.vSplits->emplace_back(VSplit(0.0f, L"1"));
  output.vSplits->emplace_back(VSplit(1.0f, L"0"));
  output.vSplits->emplace_back(VSplit(0.5f, L"\u00BD"));
}

} // namespace Xts