#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/FilterPlot.hpp>
#include <DSP/Synth/VoiceFilterDSP.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cstring>
#include <cassert>

#define FILTER_MIN_FREQ_HZ 20.0f
#define FILTER_MAX_FREQ_HZ 10000.0f

// https://cytomic.com/files/dsp/SvfLinearTrapOptimised2.pdf
// https://www.dsprelated.com/freebooks/filters/Analysis_Digital_Comb_Filter.html

namespace Xts {

PeriodicParams
FilterPlot::Params() const
{
  PeriodicParams result;
  result.periods = 5;
  result.bipolar = true;
  result.autoRange = true;
  result.allowResample = false;
  return result;
}

float
FilterPlot::Next()
{
  auto const& globalLfo = _globalLfoDsp.Next();
  auto const& cv = _cvDsp.Next(globalLfo);
  auto const& audio = _audioDsp.Next(cv);
  return _filterDsp.Next(cv, audio).Mono();
}

void
FilterPlot::Init(float bpm, float rate)
{
  new(&_cvDsp) CvDSP(&_model->voice.cv, 1.0f, bpm, rate);
  new(&_globalLfoDsp) LfoDSP(&_model->global.lfo, bpm, rate);
  new(&_audioDsp) AudioDSP(&_model->voice.audio, 4, UnitNote::C, rate);
  new(&_filterDsp) VoiceFilterDSP(&_model->voice.audio.filters[_index], _index, rate);
}

void
FilterPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  int type = static_cast<int>(model.global.plot.type);
  int index = type - static_cast<int>(PlotType::Filter1);
  VoiceFilterModel const* filter = &model.voice.audio.filters[index];
  if (filter->on) std::make_unique<FilterPlot>(&model, index)->DoRender(input, state);
}

VoiceFilterDSP::
VoiceFilterDSP(VoiceFilterModel const* model, int index, float rate):
VoiceFilterDSP()
{
  _rate = rate;
  _index = index;
  _model = model;
  _comb.x.Clear();
  _comb.y.Clear();
  _stateVar.ic1eq.Clear();
  _stateVar.ic2eq.Clear();
  _mods = TargetModsDSP(&model->mods);
}

FloatSample
VoiceFilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  if (!_model->on) return _output;
  _mods.Next(cv);
  for (int i = 0; i < _index; i++) _output += audio.filters[i] * Param::Level(_model->filterAmount[i]);
  for (int i = 0; i < XTS_VOICE_UNIT_COUNT; i++) _output += audio.units[i] * Param::Level(_model->unitAmount[i]);
  switch (_model->type)
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
  auto& s = _stateVar;
  float resBase = Param::Level(_model->resonance);
  float freqBase = Param::Level(_model->frequency);
  double res = _mods.Modulate({ resBase, false }, static_cast<int>(FilterModTarget::Resonance));
  int freq = static_cast<int>(_mods.Modulate({ freqBase, false }, static_cast<int>(FilterModTarget::Frequency)) * 255);
  float hz = Param::Frequency(freq, FILTER_MIN_FREQ_HZ, FILTER_MAX_FREQ_HZ);
  double g = std::tan(PID * hz / _rate);
  double k = 2.0 - 2.0 * res;
  double a1 = 1.0 / (1.0 + g * (g + k));
  double a2 = g * a1;
  double a3 = g * a2;
  
  auto v0 = _output.ToDouble();
  auto v3 = v0 - s.ic2eq;
  auto v1 = a1 * s.ic1eq + a2 * v3;
  auto v2 = s.ic2eq + a2 * s.ic1eq + a3 * v3;
  s.ic1eq = 2.0 * v1 - s.ic1eq;
  s.ic2eq = 2.0 * v2 - s.ic2eq;

  DoubleSample result = {};
  switch (_model->passType)
  {
  case PassType::LPF: result = v2; break;
  case PassType::BPF: result = v1; break;
  case PassType::BSF: result = v0 - k * v1; break;
  case PassType::HPF: result = v0 - k * v1 - v2; break;
  default: assert(false); break;
  }
  return result.ToFloat().Sanity();
}

FloatSample
VoiceFilterDSP::GenerateComb()
{
  auto& s = _comb;
  float minGainBase = Param::Mix(_model->combMinGain);
  float plusGainBase = Param::Mix(_model->combPlusGain);
  float minDelayBase = Param::Level(_model->combMinDelay);
  float plusDelayBase = Param::Level(_model->combPlusDelay);
  float minGain = _mods.Modulate({ minGainBase, true }, static_cast<int>(FilterModTarget::CombMinGain));
  float plusGain = _mods.Modulate({ plusGainBase, true }, static_cast<int>(FilterModTarget::CombPlusGain));
  int minDelay = static_cast<int>(_mods.Modulate({ minDelayBase, false}, static_cast<int>(FilterModTarget::CombMinDelay)) * 255);
  int plusDelay = static_cast<int>(_mods.Modulate({ plusDelayBase, false }, static_cast<int>(FilterModTarget::CombPlusDelay)) * 255);
  int minDelaySamples = static_cast<int>(Param::TimeSamplesF(minDelay, _rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  int plusDelaySamples = static_cast<int>(Param::TimeSamplesF(plusDelay, _rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  s.y.Push(_output + s.x.Get(plusDelaySamples) * plusGain + s.y.Get(minDelaySamples) * minGain);
  s.x.Push(_output);
  assert(minDelaySamples < COMB_DELAY_MAX_SAMPLES);
  assert(plusDelaySamples < COMB_DELAY_MAX_SAMPLES);
  return s.y.Get(0).Sanity();
}

} // namespace Xts