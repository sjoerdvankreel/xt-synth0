#include "DSP.hpp"
#include "CvDSP.hpp"
#include "PlotDSP.hpp"
#include "AudioDSP.hpp"
#include "FilterDSP.hpp"

#include <cstring>

#define BIQUAD_MIN_Q 0.5f
#define BIQUAD_MAX_Q 40.0f
#define BIQUAD_MIN_BW 0.5f
#define BIQUAD_MAX_BW 6.0f

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
// https://www.dsprelated.com/freebooks/filters/Analysis_Digital_Comb_Filter.html

namespace Xts
{

static void 
InitComb(FilterModel const& m, CombState& s)
{
  s.delayMin = m.dlyMin;
  s.delayPlus = m.dlyPlus;
  s.gainMin = Mix(m.gMin);
  s.gainPlus = Mix(m.gPlus);
  std::memset(&s.x, 0, sizeof(s.x));
  std::memset(&s.y, 0, sizeof(s.y));
}

static FAudioOutput
GenerateComb(FAudioOutput audio, CombState& s)
{
  s.y[0].Clear();
  s.x[0] = audio;
  s.y[0] = s.x[0] + s.x[s.delayPlus] * s.gainPlus + s.y[s.delayMin] * s.gainMin;
  for (int i = XTS_MAX_COMB_DELAY - 1; i > 0; i--)
  {
    s.x[i] = s.x[i - 1];
    s.y[i] = s.y[i - 1];
  }
  return s.y[0];
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
  double res = Level(m.res);
  double freq = FreqHz(m.freq);
  double w0 = 2.0 * PI * freq / rate;
  sinw0 = std::sin(w0);
  cosw0 = std::cos(w0);
  alpha = BiquadIsQ(m.bqType)? BiquadAlphaQ(res, sinw0): BiquadAlphaBW(res, w0, sinw0);
}

static void
InitBiquad(FilterModel const& m, float rate, BiquadState& s)
{
  double alpha;
  double sinw0;
  double cosw0;
  std::memset(&s.x, 0, sizeof(s.x));
  std::memset(&s.y, 0, sizeof(s.y));
  BiquadParameters(m, rate, sinw0, cosw0, alpha);
  switch (m.bqType)
  {
  case BiquadType::LPF: InitBiquadLPF(cosw0, alpha, s); break;
  case BiquadType::HPF: InitBiquadHPF(cosw0, alpha, s); break;
  case BiquadType::BSF: InitBiquadBSF(cosw0, alpha, s); break;
  case BiquadType::BPF: InitBiquadBPF(sinw0, cosw0, alpha, s); break;
  default: assert(false); break;
  }
  for(int i = 0; i < 3; i++) s.b[i] /= s.a[0];
  for(int i = 1; i < 3; i++) s.a[i] /= s.a[0];
}

static FAudioOutput
GenerateBiquad(FAudioOutput audio, BiquadState& s)
{
  s.y[0].Clear();
  s.x[0] = audio.ToDouble();
  s.y[0] = s.x[0] * s.b[0] + s.x[1] * s.b[1] + s.x[2] * s.b[2] - s.y[1] * s.a[1] - s.y[2] * s.a[2];
  s.x[2] = s.x[1];
  s.x[1] = s.x[0];
  s.y[2] = s.y[1];
  s.y[1] = s.y[0];
  return s.y[0].ToFloat();
}

FilterDSP::
FilterDSP(FilterModel const* model, int index, float rate) :
_index(index), _output(),
_amt1(Mix(model->amt1)),
_amt2(Mix(model->amt2)),
_units(), _flts(), _model(model),
_state()
{
  for (int i = 0; i < UnitCount; i++)
    _units[i] = Level(model->units[i]);
  for (int i = 0; i < FilterCount; i++)
    _flts[i] = Level(model->flts[i]);
  switch (model->type)
  {
  case FilterType::Comb: InitComb(*model, _state.comb); break;
  case FilterType::Bqd: InitBiquad(*model, rate, _state.biquad); break;
  default: assert(false); break;
  }
}

FAudioOutput
FilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  if (!_model->on) return _output;
  for (int i = 0; i < _index; i++) _output += audio.filts[i] * _flts[i];
  for (int i = 0; i < UnitCount; i++) _output += audio.units[i] * _units[i];
  switch (_model->type)
  {
  case FilterType::Comb: _output = GenerateComb(_output, _state.comb); break;
  case FilterType::Bqd: _output = GenerateBiquad(_output, _state.biquad); break;
  default: assert(false); break;
  }
  return _output;
}

void
FilterDSP::Plot(FilterModel const& model, CvModel const& cvModel, AudioModel const& audioModel, bool spec, int index, PlotInput const& input, PlotOutput& output)
{
  const int cycles = 5;
  if (!model.on) return;
  PlotFlags flags = PlotNone;
  flags |= PlotBipolar;
  flags |= PlotAutoRange;
  flags |= spec ? PlotSpec : 0;
  float freq = FreqNote(5 * 12 + static_cast<int>(UnitNote::C));
  auto factory = [&](float rate) 
  { 
    CvDSP cv(&cvModel, 1.0f, input.bpm, rate);
    AudioDSP audio(& audioModel, 4, UnitNote::C, rate);
    FilterDSP filter(&model, index, rate);
    return std::make_tuple(cv, audio, filter);
  };
  auto next = [](std::tuple<CvDSP, AudioDSP, FilterDSP>& state) 
  { 
    auto const& cv = std::get<CvDSP>(state).Next();
    auto const& audio = std::get<AudioDSP>(state).Next(cv);
    return std::get<FilterDSP>(state).Next(cv, audio).Mono(); 
  };
  PlotDSP::RenderCycled(cycles, freq, flags, input, output, factory, next);
}

} // namespace Xts