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
SynthDSP::Plot(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  const int plotRate = 5000;
  const int maxSamples = 5 * plotRate;

  int i = 0;
  int h = 0;
  bool l = output.channel == 0;
  int hold = TimeI(input.hold, plotRate);
  
  output.bipolar = true;
  output.rate = plotRate;
  KeyInput key(4, UnitNote::C);
  SourceInput source(plotRate, input.bpm);
  AudioInput audio(source, key);
  SynthDSP dsp(&model, &audio);
  while (i++ < maxSamples)
  {
    if (h++ == hold) dsp.Release();
    if (dsp.End()) break;
    dsp.Next();
    auto audio = dsp.Value();
    float sample = l ? audio.l : audio.r;
    output.clip |= Clip(sample);
    output.samples->push_back(sample);
  }
}

} // namespace Xts