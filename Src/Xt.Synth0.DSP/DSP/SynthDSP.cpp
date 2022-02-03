#include "DSP.hpp"
#include "SynthDSP.hpp"

namespace Xts {

void
SynthDSP::Next(SourceDSP const& source)
{
  AudioOutput output;
  for (int u = 0; u < UnitCount; u++)
  {
    _units[u].Next(source);
    output += _units[u].Value();
  }
  _global.Next(source);
  _value = output * _global.Value();
}

SynthDSP::
SynthDSP(SynthModel const* model, AudioInput const* input):
DSPBase(model, input), _source(&model->source, &input->source), 
_global(&model->global, &input->source), _units()
{
  for (int u = 0; u < UnitCount; u++)
    _units[u] = UnitDSP(&model->units[u], input);
}

void
SynthDSP::Plot(SynthModel const& model, SourceModel const& source, PlotInput const& input, PlotOutput& output)
{
  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  float plotRate = input.spec ? input.rate : 5000;
  int hold = TimeI(input.hold, plotRate);
  int maxSamples = static_cast<int>(input.spec ? input.rate : 5 * plotRate);
  
  output.max = 1.0f;
  output.min = -1.0f;
  output.rate = plotRate;
  output.vSplits->emplace_back(0.0f);

  KeyInput key(4, UnitNote::C);
  SourceInput sourceInput(plotRate, input.bpm);
  AudioInput audio(sourceInput, key);
  SynthDSP dsp(&model, &audio);
  SourceDSP sourceDsp(&source, &sourceInput);
  while (i++ < maxSamples)
  {
    if (h++ == hold) sourceDsp.Release();
    if (dsp.End(sourceDsp)) break;
    sourceDsp.Next();
    dsp.Next(sourceDsp);
    auto audio = dsp.Value();
    float sample = l ? audio.l : audio.r;
    output.clip |= Clip(sample);
    output.samples->push_back(sample);
  }
}

} // namespace Xts