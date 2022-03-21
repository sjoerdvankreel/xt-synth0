#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/FilterPlot.hpp>
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
  auto const& cv = _cvDsp.Next();
  auto const& audio = _audioDsp.Next(cv);
  return _filterDsp.Next(cv, audio).Mono();
}

void
FilterPlot::Init(float bpm, float rate)
{
  new(&_cvDsp) CvDSP(_cv, 1.0f, bpm, rate);
  new(&_filterDsp) FilterDSP(_filter, _index, rate);
  new(&_audioDsp) AudioDSP(_audio, 4, UnitNote::C, rate);
}

void
FilterPlot::Render(SynthModel const& model, PlotInput const& input, PlotState& state)
{
  int type = static_cast<int>(model.plot.type);
  int index = type - static_cast<int>(PlotType::Filter1);
  FilterModel const* filter = &model.voice.audio.filters[index];
  if (filter->on) std::make_unique<FilterPlot>(&model.voice.cv, &model.voice.audio, filter, index)->DoRender(input, state);
}

static void
InitStateVar(FilterModel const& m, StateVarState& s)
{
  s.ic1eq.Clear();
  s.ic2eq.Clear();
  s.resonance = Param::Level(m.resonance);
  s.frequency = Param::Level(m.frequency);
}

static void
InitComb(FilterModel const& m, float rate, CombState& s)
{
  s.x.Clear();
  s.y.Clear();
  s.minGain = Param::Mix(m.combMinGain);
  s.plusGain = Param::Mix(m.combPlusGain);
  s.minDelay = Param::Level(m.combMinDelay);
  s.plusDelay = Param::Level(m.combPlusDelay);
}

FilterDSP::
FilterDSP(FilterModel const* model, int index, float rate):
FilterDSP()
{
  _rate = rate;
  _index = index;
  _model = model;
  _mods = ModsDSP(model->mods);
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _unitAmount[i] = Param::Level(model->unitAmount[i]);
  for (int i = 0; i < XTS_SYNTH_FILTER_COUNT; i++) _filterAmount[i] = Param::Level(model->filterAmount[i]);
  switch (model->type)
  {
  case FilterType::Comb: InitComb(*model, rate, _state.comb); break;
  case FilterType::StateVar: InitStateVar(*model, _state.stateVar); break;
  default: assert(false); break;
  }
}

FloatSample
FilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  if (!_model->on) return _output;
  _mods.Next(cv);
  for (int i = 0; i < _index; i++) _output += audio.filters[i] * _filterAmount[i];
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _output += audio.units[i] * _unitAmount[i];
  switch (_model->type)
  {
  case FilterType::Comb: _output = GenerateComb(); break;
  case FilterType::StateVar: _output = GenerateStateVar(); break;
  default: assert(false); break;
  }
  return _output.Sanity();
}

FloatSample
FilterDSP::GenerateStateVar()
{
  auto& s = _state.stateVar;
  double res = _mods.Modulate(FilterModTarget::Resonance, { s.resonance, false });
  int freq = static_cast<int>(_mods.Modulate(FilterModTarget::Frequency, { s.frequency, false }) * 255);
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
FilterDSP::GenerateComb()
{
  auto& s = _state.comb;
  float minGain = _mods.Modulate(FilterModTarget::CombMinGain, { s.minGain , true });
  float plusGain = _mods.Modulate(FilterModTarget::CombPlusGain, { s.plusGain , true });
  int minDelay = static_cast<int>(_mods.Modulate(FilterModTarget::CombMinDelay, {s.minDelay, false}) * 255);
  int plusDelay = static_cast<int>(_mods.Modulate(FilterModTarget::CombPlusDelay, { s.plusDelay, false }) * 255);
  int minDelaySamples = static_cast<int>(Param::TimeSamplesF(minDelay, _rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  int plusDelaySamples = static_cast<int>(Param::TimeSamplesF(plusDelay, _rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  s.y.Push(_output + s.x.Get(plusDelaySamples) * plusGain + s.y.Get(minDelaySamples) * minGain);
  s.x.Push(_output);
  assert(minDelaySamples < COMB_DELAY_MAX_SAMPLES);
  assert(plusDelaySamples < COMB_DELAY_MAX_SAMPLES);
  return s.y.Get(0).Sanity();
}

} // namespace Xts