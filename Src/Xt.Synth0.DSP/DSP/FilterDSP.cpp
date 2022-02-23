#include "FilterDSP.hpp"
#include "DSP.hpp"

namespace Xts {

static const float MinQ = 0.5f;
static const float MaxQ = 100.0f;
static const float MinBW = 0.5f;
static const float MaxBW = 6.0f;

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
FilterDSP::
FilterDSP(FilterModel const* model, int index, float rate) :
_index(index), _a(), _b(), _x(), _y(),
_model(model), _units(), _flts(), 
_rate(rate),
_amt1(Mix(model->amt1)),
_amt2(Mix(model->amt2))
{
  for (int i = 0; i < 3; i++)
    _x[i].Clear();
  for (int i = 0; i < 3; i++)
    _y[i].Clear();
  for (int i = 0; i < UnitCount; i++)
    _units[i] = Level(model->units[i]);
  for (int i = 0; i < FilterCount; i++)
    _flts[i] = Level(model->flts[i]);

  float freq = FreqHz(model->freq);
  float w0 = 2.0f * PI * freq / _rate;
  float sinw0 = std::sinf(w0);
  float cosw0 = std::cosf(w0);

  float q = MinQ + Level(model->res) * (MaxQ - MinQ);
  float alphaQ = sinw0 / (2.0f * q);
  float bw = (1.0f - Level(model->res)) * (MaxBW - MinBW);
  float alphaBW = sinw0 * std::sinhf(std::logf(2.0f) / 2.0f * bw * w0 / sinw0);

  switch (model->type)
  {
  case FilterType::LPF:
    _a[0] = 1.0f + alphaQ;
    _a[1] = -2.0f * cosw0;
    _a[2] = 1.0f - alphaQ;
    _b[0] = (1.0f - cosw0) / 2.0f;
    _b[1] = 1.0f - cosw0;
    _b[2] = (1.0f - cosw0) / 2.0f;
    break;
  case FilterType::HPF:
    _a[0] = 1.0f + alphaQ;
    _a[1] = -2.0f * cosw0;
    _a[2] = 1.0f - alphaQ;
    _b[0] = (1.0f + cosw0) / 2.0f;
    _b[1] = -(1.0f + cosw0);
    _b[2] = (1.0f + cosw0) / 2.0f;
    break;
  case FilterType::BPF:
    _a[0] = 1.0f + alphaBW;
    _a[1] = -2.0f * cosw0;
    _a[2] = 1.0f - alphaBW;
    _b[0] = sinw0 / 2.0f;
    _b[1] = 0.0f;
    _b[2] = -sinw0 / 2.0f;
    break;
  case FilterType::BSF:
    _a[0] = 1.0f + alphaBW;
    _a[1] = -2.0f * cosw0;
    _a[2] = 1.0f - alphaBW;
    _b[0] = 1.0f;
    _b[1] = -2.0f * cosw0;
    _b[2] = 1.0f;
    break;
  default:
    assert(false);
    break;
  }
  
  _a[1] /= _a[0];
  _a[2] /= _a[0];
  _b[0] /= _a[0];
  _b[1] /= _a[0];
  _b[2] /= _a[0];
}

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
AudioOutput 
FilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _y[0].Clear();
  if(!_model->on) return _y[0];
  _x[0].Clear();
  for(int i = 0; i < UnitCount; i++)
    _x[0] += audio.units[i] * _units[i];
  for(int i = 0; i < _index; i++)
    _x[0] += audio.filts[i] * _flts[i];
  _y[0] = _x[0] * _b[0] + _x[1] * _b[1] + _x[2] * _b[2] - _y[1] * _a[1] - _y[2] * _a[2];
  _x[2] = _x[1];
  _x[1] = _x[0];
  _y[2] = _y[1];
  _y[1] = _y[0];
  return _y[0];
}

} // namespace Xts