#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/Utility.hpp>
#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/FilterPlot.hpp>
#include <Model/Synth/SynthModel.hpp>

#include <memory>
#include <cstring>
#include <cassert>

#define BIQUAD_MIN_Q 0.5f
#define BIQUAD_MAX_Q 40.0f
#define BIQUAD_MIN_BW 0.5f
#define BIQUAD_MAX_BW 6.0f

#define BIQUAD_MIN_FREQ_HZ 20.0f
#define BIQUAD_MAX_FREQ_HZ 10000.0f

// https://cytomic.com/files/dsp/SvfLinearTrapOptimised2.pdf
// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
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
  FilterModel const* filter = &model.audio.filters[index];
  if (filter->on) std::make_unique<FilterPlot>(&model.cv, &model.audio, filter, index)->DoRender(input, state);
}

static void
InitBiquadLPF(double cosw0, double alpha, BiquadState& s)
{
  s.a[0] = 1.0 + alpha;
  s.a[1] = -2.0 * cosw0;
  s.a[2] = 1.0 - alpha;
  s.b[0] = (1.0 - cosw0) / 2.0;
  s.b[1] = 1.0 - cosw0;
  s.b[2] = (1.0 - cosw0) / 2.0;
}

static void
InitBiquadHPF(double cosw0, double alpha, BiquadState& s)
{
  s.a[0] = 1.0 + alpha;
  s.a[1] = -2.0 * cosw0;
  s.a[2] = 1.0 - alpha;
  s.b[0] = (1.0 + cosw0) / 2.0;
  s.b[1] = -(1.0 + cosw0);
  s.b[2] = (1.0 + cosw0) / 2.0;
}

static void
InitBiquadBSF(double cosw0, double alpha, BiquadState& s)
{
  s.a[0] = 1.0 + alpha;
  s.a[1] = -2.0 * cosw0;
  s.a[2] = 1.0 - alpha;
  s.b[0] = 1.0;
  s.b[1] = -2.0 * cosw0;
  s.b[2] = 1.0;
}

static void
InitBiquadBPF(double sinw0, double cosw0, double alpha, BiquadState& s)
{
  s.a[0] = 1.0 + alpha;
  s.a[1] = -2.0 * cosw0;
  s.a[2] = 1.0 - alpha;
  s.b[0] = sinw0 / 2.0;
  s.b[1] = 0.0;
  s.b[2] = -sinw0 / 2.0;
}

static bool
BiquadIsQ(BiquadType type)
{
  switch (type)
  {
  case BiquadType::LPF: case BiquadType::HPF: return true;
  case BiquadType::BPF: case BiquadType::BSF: return false;
  default: assert(false); return false;
  }
}

static double
BiquadAlphaQ(double res, double sinw0)
{
  double q = BIQUAD_MIN_Q + res * (BIQUAD_MAX_Q - BIQUAD_MIN_Q);
  return sinw0 / (2.0 * q);
}

static double
BiquadAlphaBW(double res, double w0, double sinw0)
{
  double q = (1.0 - res) * (BIQUAD_MAX_BW - BIQUAD_MIN_BW);
  return sinw0 * std::sinh(std::log(2.0) / 2.0 * q * w0 / sinw0);
}

static void
BiquadParameters(FilterModel const& m, float rate, double& sinw0, double& cosw0, double& alpha)
{
  double res = Param::Level(m.biquadResonance);
  double freq = Param::Frequency(m.biquadFrequency, BIQUAD_MIN_FREQ_HZ, BIQUAD_MAX_FREQ_HZ);
  double w0 = 2.0 * PID * freq / rate;
  sinw0 = std::sin(w0);
  cosw0 = std::cos(w0);
  alpha = BiquadIsQ(m.biquadType)? BiquadAlphaQ(res, sinw0): BiquadAlphaBW(res, w0, sinw0);
}

static void
InitBiquad(FilterModel const& m, float rate, BiquadState& s)
{
  double alpha;
  double sinw0;
  double cosw0;
  s.x.Clear();
  s.y.Clear();
  BiquadParameters(m, rate, sinw0, cosw0, alpha);
  switch (m.biquadType)
  {
  case BiquadType::LPF: InitBiquadLPF(cosw0, alpha, s); break;
  case BiquadType::HPF: InitBiquadHPF(cosw0, alpha, s); break;
  case BiquadType::BSF: InitBiquadBSF(cosw0, alpha, s); break;
  case BiquadType::BPF: InitBiquadBPF(sinw0, cosw0, alpha, s); break;
  default: assert(false); break;
  }
  for(int i = 0; i < 3; i++) s.b[i] = Sanity(s.b[i] / s.a[0]);
  for(int i = 1; i < 3; i++) s.a[i] = Sanity(s.a[i] / s.a[0]);
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
  case FilterType::Biquad: InitBiquad(*model, rate, _state.biquad); break;
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
  case FilterType::Biquad: _output = GenerateBiquad(); break;
  default: assert(false); break;
  }
  return _output.Sanity();
}

FloatSample
FilterDSP::GenerateBiquad()
{
  auto& s = _state.biquad;
  s.x.Push(_output.ToDouble());
  s.y.Push(s.x.Get(0) * s.b[0] + s.x.Get(1) * s.b[1] + s.x.Get(2) * s.b[2] - s.y.Get(0) * s.a[1] - s.y.Get(1) * s.a[2]);
  return s.y.Get(0).ToFloat().Sanity();
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