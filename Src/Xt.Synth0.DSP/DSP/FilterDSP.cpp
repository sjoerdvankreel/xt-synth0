#include "DSP.hpp"
#include "CvDSP.hpp"
#include "PlotDSP.hpp"
#include "AudioDSP.hpp"
#include "FilterDSP.hpp"

namespace Xts {

static const float MinQ = 0.5f;
static const float MaxQ = 40.0f;
static const float MinBW = 0.5f;
static const float MaxBW = 6.0f;

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
FilterDSP::
FilterDSP(FilterModel const* model, int index, float rate) :
_output(),
_index(index),
_amt1(Mix(model->amt1)),
_amt2(Mix(model->amt2)),
_units(), _flts(), _model(model),
_cbdPlus(model->dlyPlus),
_cbdMin(model->dlyMin),
_cbgPlus(Mix(model->gPlus)),
_cbgMin(Mix(model->gMin)),
_bqa(), _bqb(), _bqx(), _bqy(), _cbx(), _cby()
{
  for (int i = 0; i < UnitCount; i++)
    _units[i] = Level(model->units[i]);
  for (int i = 0; i < FilterCount; i++)
    _flts[i] = Level(model->flts[i]);
  switch (model->type)
  {
  case FilterType::Comb: InitComb(); break;
  case FilterType::Bqd: InitBQ(rate); break;
  default: assert(false); break;
  }
}

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
AudioOutput
FilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  if (!_model->on) return _output;
  for (int i = 0; i < UnitCount; i++)
    _output += audio.units[i] * _units[i];
  for (int i = 0; i < _index; i++)
    _output += audio.filts[i] * _flts[i];
  switch (_model->type)
  {
  case FilterType::Bqd: _output = GenerateBQ(_output); break;
  case FilterType::Comb: _output = GenerateComb(_output); break;
  default: assert(false); break;
  }
  return _output;
}

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
AudioOutput
FilterDSP::GenerateBQ(AudioOutput audio)
{
  _bqy[0].Clear();
  _bqx[0] = audio;
  _bqy[0] = _bqx[0] * _bqb[0] + _bqx[1] * _bqb[1] + _bqx[2] * _bqb[2] - _bqy[1] * _bqa[1] - _bqy[2] * _bqa[2];
  _bqx[2] = _bqx[1];
  _bqx[1] = _bqx[0];
  _bqy[2] = _bqy[1];
  _bqy[1] = _bqy[0];
  return _bqy[0];
}

// https://www.dsprelated.com/freebooks/filters/Analysis_Digital_Comb_Filter.html
AudioOutput
FilterDSP::GenerateComb(AudioOutput audio)
{
  return audio;
}

void 
FilterDSP::InitComb()
{
  for (int i = 0; i < 16; i++)
  {
    _cbx[i].Clear();
    _cby[i].Clear();
  }
}

void
FilterDSP::InitBQ(float rate)
{
  for (int i = 0; i < 3; i++)
  {
    _bqx[i].Clear();
    _bqy[i].Clear();
  }

  float freq = FreqHz(_model->freq);
  float w0 = 2.0f * PI * freq / rate;
  float sinw0 = std::sinf(w0);
  float cosw0 = std::cosf(w0);

  float res = Level(_model->res);
  float q = MinQ + res * (MaxQ - MinQ);
  float alphaQ = sinw0 / (2.0f * q);
  float bw = (1.0f - res) * (MaxBW - MinBW);
  float alphaBW = sinw0 * std::sinhf(std::logf(2.0f) / 2.0f * bw * w0 / sinw0);

  switch (_model->bqType)
  {
  case BiquadType::LPF:
    _bqa[0] = 1.0f + alphaQ;
    _bqa[1] = -2.0f * cosw0;
    _bqa[2] = 1.0f - alphaQ;
    _bqb[0] = (1.0f - cosw0) / 2.0f;
    _bqb[1] = 1.0f - cosw0;
    _bqb[2] = (1.0f - cosw0) / 2.0f;
    break;
  case BiquadType::HPF:
    _bqa[0] = 1.0f + alphaQ;
    _bqa[1] = -2.0f * cosw0;
    _bqa[2] = 1.0f - alphaQ;
    _bqb[0] = (1.0f + cosw0) / 2.0f;
    _bqb[1] = -(1.0f + cosw0);
    _bqb[2] = (1.0f + cosw0) / 2.0f;
    break;
  case BiquadType::BPF:
    _bqa[0] = 1.0f + alphaBW;
    _bqa[1] = -2.0f * cosw0;
    _bqa[2] = 1.0f - alphaBW;
    _bqb[0] = sinw0 / 2.0f;
    _bqb[1] = 0.0f;
    _bqb[2] = -sinw0 / 2.0f;
    break;
  case BiquadType::BSF:
    _bqa[0] = 1.0f + alphaBW;
    _bqa[1] = -2.0f * cosw0;
    _bqa[2] = 1.0f - alphaBW;
    _bqb[0] = 1.0f;
    _bqb[1] = -2.0f * cosw0;
    _bqb[2] = 1.0f;
    break;
  default:
    assert(false);
    break;
  }

  _bqa[1] /= _bqa[0];
  _bqa[2] /= _bqa[0];
  _bqb[0] /= _bqa[0];
  _bqb[1] /= _bqa[0];
  _bqb[2] /= _bqa[0];
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