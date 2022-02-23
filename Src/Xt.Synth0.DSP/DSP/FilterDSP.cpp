#include "FilterDSP.hpp"
#include "DSP.hpp"

namespace Xts {

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
FilterDSP::
FilterDSP(FilterModel const* model, float rate) :
_a(), _b(), _output(), _x(), _y(),
_model(model), _units(), _flts(), 
_rate(rate),
_amt1(Mix(model->amt1)),
_amt2(Mix(model->amt2))
{
  for (int i = 0; i < 3; i++)
  {
    _x[i].Clear();
    _y[i].Clear();
  }
  for (int i = 0; i < UnitCount; i++)
    _units[i] = Level(model->units[i]);
  for (int i = 0; i < FilterCount - 1; i++)
    _flts[i] = Level(model->flts[i]);


  float freq = FreqHz(model->freq);
  float q = Level(model->res) * 100.0f + 0.5f;
  float const omega = 2.0f * PI * freq / _rate;
  float const alpha = std::sin(omega) / (2.0f * q);

  _b[2] = _b[0] = (1 + std::cos(omega)) / 2.0f;
  _b[1] = -(1 + std::cos(omega));
  _a[0] = 1.0f + alpha;
  _a[1] = -2.0f * std::cos(omega);
  _a[2] = 1.0f - alpha;

  _b[0] /= _a[0]; _b[1] /= _a[0]; _b[2] /= _a[0]; _a[1] /= _a[0]; _a[2] /= _a[0];

/*
  float a0;
  float freq = FreqHz(model->freq);
  // todo find out range of Q (0.5-50?)
  float res = Level(model->res)*100.0f + 0.5f;
  float w0 = 2.0f * PI * freq / rate;
  float cosw0 = std::cosf(w0);
  float sinw0 = std::sinf(w0);
  float alpha = sinw0 / (2.0f * res);
  
  // todo case lpf
  switch(model->type)
  {
  case FilterType::LPF:
    a0 = 1.0f + alpha;
    _a[0] = (-2.0f * cosw0) / a0;
    _a[1] = (1.0f - alpha) / a0;
    _b[1] = (1.0f - cosw0) / a0;
    _b[0] = _b[2] = ((1.0f - cosw0) / 2.0f) / a0;
    break;
  case FilterType::HPF:
    a0 = 1.0f + alpha;
    _a[0] = (-2.0f * cosw0) / a0;
    _a[1] = (1.0f - alpha) / a0;
    _b[1] = (-(1.0f + cosw0)) / a0;
    _b[0] = _b[2] = ((1.0f + cosw0) / 2.0f) / a0;
    break;
  default:
    assert(false); 
    break;
  }

  assert(!std::isnan(_a[0]));
  assert(!std::isnan(_a[1]));
  assert(!std::isnan(_b[0]));
  assert(!std::isnan(_b[1]));
  assert(!std::isnan(_b[2]));
*/
}

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
AudioOutput 
FilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _x[0].l = audio.units[0].l;
  float const out =
    _b[0] * _x[0].l +
    _b[1] * _x[1].l +
    _b[2] * _x[2].l -
    _a[1] * _y[1].l -
    _a[2] * _y[2].l;

  _x[2] = _x[1];
  _x[1] = _x[0];
  _y[2] = _y[1];
  _y[1] = { out, out };

  return { out, out };

/*
  _output.Clear();
  if (!_model->on) return Output();
  for(int i = 0; i < UnitCount; i++)
    _output += audio.units[i] * _units[i];
  //for(int i = 0; i < FilterCount - 1; i++)
  //  _output += audio.filters[]
  _output = _output * _b[0] + _x[1] * _b[1] + _x[0] * _b[2] - _y[1] * _a[0] - _y[0] * _a[1];
  _x[0] = _x[1];
  _x[1] = audio.units[0];
  _y[0] = _y[1];
  _y[1] = _output;
  assert(!std::isnan(_output.l));
  assert(!std::isnan(_output.r));
  assert(!std::isinf(_output.l));
  assert(!std::isinf(_output.r));
  return _output;
*/
}

} // namespace Xts