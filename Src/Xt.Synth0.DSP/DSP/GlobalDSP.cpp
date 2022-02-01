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
GlobalDSP::Plot(GlobalModel const& model, PlotInput const& input, PlotOutput& output)
{
  const int plotRate = 5000;
  const int maxSamples = 5 * plotRate;

  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  int hold = TimeI(input.hold, plotRate);

  output.bipolar = true;
  output.rate = plotRate;
  SourceModel sourceModel = SourceModel();
  SourceInput sourceInput(plotRate, input.bpm);
  SourceDSP sourceDSP(&sourceModel, &sourceInput);
  GlobalDSP dsp(&model, &sourceInput);
  while (i++ < maxSamples)
  {
    if (h++ == hold) sourceDSP.Release();
    if (dsp.End(sourceDSP)) break;
    sourceDSP.Next();
    dsp.Next(sourceDSP);
    output.samples->push_back(dsp.Value());
  }
}

} // namespace Xts