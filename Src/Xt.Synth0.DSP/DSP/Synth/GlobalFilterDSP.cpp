#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/GlobalFilterDSP.hpp>
#include <DSP/Synth/GlobalFilterPlot.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cstring>
#include <cassert>

namespace Xts {

void
GlobalFilterPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{ std::make_unique<GlobalFilterPlot>(&model)->DoRender(input, state); }

GlobalFilterDSP::
GlobalFilterDSP(GlobalFilterModel const* model, float rate):
GlobalFilterDSP()
{
  _output.Clear();
  _model = model;
  _mod = GlobalModDSP(&_model->mod);
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
  case FilterType::Ladder: case FilterType::StateVar: _output = Generate(globalLfo); break;
  default: assert(false); break;
  }
  return _output.Sanity();
}

FloatSample
GlobalFilterDSP::Generate(CvSample globalLfo)
{
  float resBase = Param::Level(_model->filter.resonance);
  float freqBase = Param::Level(_model->filter.frequency);
  float resonance = _mod.Modulate(globalLfo, { resBase, false }, static_cast<int>(FilterModTarget::Resonance));
  float freq = _mod.Modulate(globalLfo, { freqBase, false }, static_cast<int>(FilterModTarget::Frequency)) * 256.0f;
  float hz = Param::Frequency(freq, XTS_STATE_VAR_MIN_FREQ_HZ, XTS_STATE_VAR_MAX_FREQ_HZ);
  switch (_model->filter.type)
  {
  case FilterType::Ladder: return _filter.GenerateLadder(_output, hz, resonance);
  case FilterType::StateVar: return _filter.GenerateStateVar(_output, hz, resonance);
  default: assert(false); return FloatSample();
  }
}

FloatSample
GlobalFilterDSP::GenerateComb(CvSample globalLfo)
{
  float minGainBase = Param::Mix(_model->filter.combMinGain);
  float plusGainBase = Param::Mix(_model->filter.combPlusGain);
  float minDelayBase = Param::Level(_model->filter.combMinDelay);
  float plusDelayBase = Param::Level(_model->filter.combPlusDelay);
  float minGain = _mod.Modulate(globalLfo, { minGainBase, true }, static_cast<int>(FilterModTarget::CombMinGain));
  float plusGain = _mod.Modulate(globalLfo, { plusGainBase, true }, static_cast<int>(FilterModTarget::CombPlusGain));
  int minDelay = static_cast<int>(_mod.Modulate(globalLfo, { minDelayBase, false }, static_cast<int>(FilterModTarget::CombMinDelay)) * 255);
  int plusDelay = static_cast<int>(_mod.Modulate(globalLfo, { plusDelayBase, false }, static_cast<int>(FilterModTarget::CombPlusDelay)) * 255);
  return _filter.GenerateComb(_output, minDelay, plusDelay, minGain, plusGain);
}

} // namespace Xts