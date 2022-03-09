#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/FilterPlot.hpp>
#include <DSP/Param.hpp>
#include <DSP/Utility.hpp>

#include <memory>
#include <cstring>
#include <cassert>

#define BIQUAD_MIN_Q 0.5f
#define BIQUAD_MAX_Q 40.0f
#define BIQUAD_MIN_BW 0.5f
#define BIQUAD_MAX_BW 6.0f

#define BIQUAD_MIN_FREQ_HZ 20.0f
#define BIQUAD_MAX_FREQ_HZ 10000.0f

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
FilterPlot::Render(SynthModel const& model, PlotInput const& input, PlotOutput& output)
{
  int type = static_cast<int>(model.plot.type);
  int index = type - static_cast<int>(PlotType::Filt1);
  FilterModel const* filter = &model.audio.filters[index];
  if (filter->on) std::make_unique<FilterPlot>(&model.cv, &model.audio, filter, index)->RenderCore(input, output);
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

static FloatSample
GenerateBiquad(FloatSample audio, BiquadState& s)
{
  s.x.Push(audio.ToDouble());
  s.y.Push(s.x.Get(0) * s.b[0] + s.x.Get(1) * s.b[1] + s.x.Get(2) * s.b[2] - s.y.Get(0) * s.a[1] - s.y.Get(1) * s.a[2]);
  return s.y.Get(0).ToFloat().Sanity();
}

static void
InitComb(FilterModel const& m, float rate, CombState& s)
{
  s.x.Clear();
  s.y.Clear();
  s.minGain = Param::Mix(m.combMinGain);
  s.plusGain = Param::Mix(m.combPlusGain);
  s.minDelay = static_cast<int>(Param::TimeSamplesF(m.combMinDelay, rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  s.plusDelay = static_cast<int>(Param::TimeSamplesF(m.combPlusDelay, rate, XTS_COMB_MIN_DELAY_MS, XTS_COMB_MAX_DELAY_MS));
  assert(s.minDelay < COMB_DELAY_MAX_SAMPLES);
  assert(s.plusDelay < COMB_DELAY_MAX_SAMPLES);
}

static FloatSample
GenerateComb(FloatSample audio, CombState& s)
{
  s.y.Push(audio + s.x.Get(s.plusDelay) * s.plusGain + s.y.Get(s.minDelay) * s.minGain);
  s.x.Push(audio);
  return s.y.Get(0).Sanity();
}

FilterDSP::
FilterDSP(FilterModel const* model, int index, float rate):
FilterDSP()
{
  _index = index;
  _model = model;
  _modAmount1 = Param::Mix(model->mod1.amount);
  _modAmount2 = Param::Mix(model->mod2.amount);
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
  for (int i = 0; i < _index; i++) _output += audio.filters[i] * _filterAmount[i];
  for (int i = 0; i < XTS_SYNTH_UNIT_COUNT; i++) _output += audio.units[i] * _unitAmount[i];
  switch (_model->type)
  {
  case FilterType::Comb: _output = GenerateComb(_output, _state.comb); break;
  case FilterType::Biquad: _output = GenerateBiquad(_output, _state.biquad); break;
  default: assert(false); break;
  }
  return _output.Sanity();
}

} // namespace Xts