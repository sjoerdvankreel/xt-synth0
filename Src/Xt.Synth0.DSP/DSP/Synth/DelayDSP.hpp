#ifndef XTS_DSP_SYNTH_DELAY_DSP_HPP
#define XTS_DSP_SYNTH_DELAY_DSP_HPP

#include <DSP/Shared/Config.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <DSP/Shared/DelayBuffer.hpp>

#define XTS_DELAY_TIME_MIN_MS 1.0f
#define XTS_DELAY_TIME_MAX_MS 2000.0f

namespace Xts {

static constexpr int DELAY_MAX_SAMPLES = 
static_cast<int>(XTS_DELAY_TIME_MAX_MS * XTS_MAX_SAMPLE_RATE / 1000.0f + 1.0f);

class DelayDSP
{
  float _bpm;
  float _rate;
  FloatSample _output;
  struct DelayModel const* _model;
  DelayBuffer<FloatSample, DELAY_MAX_SAMPLES> _line;
public:
  FloatSample Next(FloatSample x);
  FloatSample Output() const { return _output; };
public:
  DelayDSP() = default;
  DelayDSP(struct DelayModel const* model, float bpm, float rate): 
  _model(model), _bpm(bpm), _rate(rate) { _line.Clear(); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_DELAY_DSP_HPP