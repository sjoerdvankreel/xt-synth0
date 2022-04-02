#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/VoiceFilterDSP.hpp>
#include <DSP/Synth/VoiceFilterPlot.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cstring>
#include <cassert>

namespace Xts {

PeriodicParams
VoiceFilterPlot::Params() const
{
  PeriodicParams result;
  result.periods = 5;
  result.bipolar = true;
  result.autoRange = true;
  result.allowResample = false;
  return result;
}

float
VoiceFilterPlot::Next()
{
  auto const& globalLfo = _globalLfoDsp.Next();
  auto const& cv = _cvDsp.Next(globalLfo);
  auto const& audio = _audioDsp.Next(cv);
  return _filterDsp.Next(cv, audio).Mono();
}

void
VoiceFilterPlot::Init(float bpm, float rate)
{
  new(&_cvDsp) CvDSP(&_model->voice.cv, 1.0f, bpm, rate);
  new(&_globalLfoDsp) LfoDSP(&_model->global.lfo, bpm, rate);
  new(&_audioDsp) AudioDSP(&_model->voice.audio, 4, NoteType::C, rate);
  new(&_filterDsp) VoiceFilterDSP(&_model->voice.audio.filters[_index], _index, rate);
}

void
VoiceFilterPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  int type = static_cast<int>(model.global.plot.type);
  int index = type - static_cast<int>(PlotType::Filter1);
  VoiceFilterModel const* filter = &model.voice.audio.filters[index];
  if (filter->filter.on) std::make_unique<VoiceFilterPlot>(&model, index)->DoRender(input, state);
}

VoiceFilterDSP::
VoiceFilterDSP(VoiceFilterModel const* model, int index, float rate):
VoiceFilterDSP()
{
  _output.Clear();
  _index = index;
  _model = model;
  _mods = TargetModsDSP(&model->mods);
  _dsp = FilterDSP(&model->filter, rate);
}

FloatSample
VoiceFilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  if (!_model->filter.on) return _output;
  _mods.Next(cv);
  for (int i = 0; i < _index; i++) _output += audio.filters[i] * Param::Level(_model->filterAmount[i]);
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _output += audio.units[i] * Param::Level(_model->unitAmount[i]);
  switch (_model->filter.type)
  {
  case FilterType::Comb: _output = GenerateComb(); break;
  case FilterType::StateVar: _output = GenerateStateVar(); break;
  default: assert(false); break;
  }
  return _output.Sanity();
}

FloatSample
VoiceFilterDSP::GenerateStateVar()
{
  float resBase = Param::Level(_model->filter.resonance);
  float freqBase = Param::Level(_model->filter.frequency);
  double resonance = _mods.Modulate({ resBase, false }, static_cast<int>(FilterModTarget::Resonance));
  int freq = static_cast<int>(_mods.Modulate({ freqBase, false }, static_cast<int>(FilterModTarget::Frequency)) * 255);
  return _dsp.GenerateStateVar(_output, freq, resonance);
}

FloatSample
VoiceFilterDSP::GenerateComb()
{
  float minGainBase = Param::Mix(_model->filter.combMinGain);
  float plusGainBase = Param::Mix(_model->filter.combPlusGain);
  float minDelayBase = Param::Level(_model->filter.combMinDelay);
  float plusDelayBase = Param::Level(_model->filter.combPlusDelay);
  float minGain = _mods.Modulate({ minGainBase, true }, static_cast<int>(FilterModTarget::CombMinGain));
  float plusGain = _mods.Modulate({ plusGainBase, true }, static_cast<int>(FilterModTarget::CombPlusGain));
  int minDelay = static_cast<int>(_mods.Modulate({ minDelayBase, false}, static_cast<int>(FilterModTarget::CombMinDelay)) * 255);
  int plusDelay = static_cast<int>(_mods.Modulate({ plusDelayBase, false }, static_cast<int>(FilterModTarget::CombPlusDelay)) * 255);
  return _dsp.GenerateComb(_output, minDelay, plusDelay, minGain, plusGain);
}

} // namespace Xts