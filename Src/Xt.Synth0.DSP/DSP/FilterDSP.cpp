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
  _x[0].Clear();
  _x[1].Clear();
  _y[0].Clear();
  _y[1].Clear();
  for (int i = 0; i < UnitCount; i++)
    _units[i] = Level(model->units[i]);
  for (int i = 0; i < FilterCount; i++)
    _flts[i] = Level(model->flts[i]);

  float freq = FreqHz(model->freq);
  // todo find out range of Q
  float res = Level(model->res) + 0.5f;
  float w0 = 2.0f * PI * freq / rate;
  float cosw0 = std::cosf(w0);
  //float sinw0 = std::sinf(w0);
  float alpha = std::sinf(w0) / (2.0f * res);
  // todo case lpf
  float a0 = 1.0f + alpha;
  _a[0] = (-2.0f * cosw0) / a0;
  _a[1] = (1.0f - alpha) / a0;
  _b[1] = (1.0f - cosw0) / a0;
  _b[0] = _b[2] = ((1.0f - cosw0) / 2.0f) / a0;
}

// https://www.musicdsp.org/en/latest/Filters/197-rbj-audio-eq-cookbook.html
AudioOutput 
FilterDSP::Next(CvState const& cv, AudioState const& audio)
{
  _output.Clear();
  if (!_model->on) return Output();
  _output += audio.units[0] * _b[0] + _x[1] * _b[1] + _x[0] * _b[2];
  _output += _y[1] * _a[0] + _y[0] * _a[1];
  _x[0] = _x[1];
  _x[1] = audio.units[0];
  _y[0] = _y[1];
  _y[1] = _output;
  return _output;
}

} // namespace Xts