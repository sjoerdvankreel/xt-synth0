#include "DSP.hpp"
#include "GlobalDSP.hpp"

namespace Xts {

void
GlobalDSP::Next(SourceDSP const& source)
{
  float amp = Level(_model->amp);
  float lfoAmt = Level(_model->ampLfoAmt);
  float lfoVal = source.Lfos()[static_cast<int>(_model->ampLfo)].Value();
  float lfo = (1.0f - lfoAmt) + lfoAmt * lfoVal;
  float envAmt = Level(_model->ampEnvAmt);
  float envVal = source.Envs()[static_cast<int>(_model->ampEnv)].Value();
  float env = envAmt * envVal;
  _value = (amp * lfo) + (1.0f - amp) * env * lfo;
}

void
GlobalDSP::Plot(GlobalModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output)
{
  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  float plotRate = input.spec? input.rate: 5000;
  int hold = TimeI(input.hold, plotRate);  
  int maxSamples = static_cast<int>(input.spec? input.rate: 5 * plotRate);
  bool bipolar = source.lfos[static_cast<int>(model.ampLfo)].bi != XtsFalse;
  output.max = 1.0f;
  output.rate = plotRate;
  output.min = bipolar ? -1.0f: 0.0f;
  SourceInput sourceInput(plotRate, input.bpm);
  GlobalDSP dsp(&model, &sourceInput);
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
  if (bipolar)
  {
    output.vSplits->emplace_back(VSplit(0.0f, L"0"));
    output.vSplits->emplace_back(VSplit(1.0f, L"-1"));
    output.vSplits->emplace_back(VSplit(-1.0f, L"1"));
  }
  else
  {
    output.vSplits->emplace_back(VSplit(0.0f, L"1"));
    output.vSplits->emplace_back(VSplit(1.0f, L"0"));
    output.vSplits->emplace_back(VSplit(0.5f, L"\u00BD"));
  }
}

} // namespace Xts