#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/GlobalFilterDSP.hpp>
#include <DSP/Synth/GlobalFilterPlot.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cstring>
#include <cassert>

namespace Xts {

GlobalFilterDSP::
GlobalFilterDSP(GlobalFilterModel const* model, float rate):
GlobalFilterDSP()
{
  _output.Clear();
  _model = model;
  _filter = FilterDSP(&model->filter, rate);
}

FloatSample
GlobalFilterDSP::Next(CvSample globalLfo, FloatSample x)
{
  _output = x;
  if (!_model->filter.on) return _output;
  switch (_model->filter.type)
  {
  case FilterType::Comb: _output = GenerateComb(globalLfo); break;
  case FilterType::StateVar: _output = GenerateStateVar(globalLfo); break;
  default: assert(false); break;
  }
  return _output.Sanity();
}

FloatSample
GlobalFilterDSP::GenerateStateVar(CvSample globalLfo)
{
  float resBase = Param::Level(_model->filter.resonance);
  float freqBase = Param::Level(_model->filter.frequency);
  double resonance = resBase; // TODO
  int freq = static_cast<int>(freqBase * 255); // TODO
  return _filter.GenerateStateVar(_output, freq, resonance);
}

FloatSample
GlobalFilterDSP::GenerateComb(CvSample globalLfo)
{
  float minGainBase = Param::Mix(_model->filter.combMinGain);
  float plusGainBase = Param::Mix(_model->filter.combPlusGain);
  float minDelayBase = Param::Level(_model->filter.combMinDelay);
  float plusDelayBase = Param::Level(_model->filter.combPlusDelay);
  float minGain = minGainBase; // TODO
  float plusGain = plusGainBase; // TODO
  int minDelay = static_cast<int>(minDelayBase * 255);
  int plusDelay = static_cast<int>(plusDelayBase * 255);
  return _filter.GenerateComb(_output, minDelay, plusDelay, minGain, plusGain);
}

} // namespace Xts