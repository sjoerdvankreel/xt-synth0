#ifndef XTS_DSP_SYNTH_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_FILTER_DSP_HPP

#include <DSP/Shared/Config.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/DelayBuffer.hpp>
#include <DSP/Shared/AudioSample.hpp>

#define XTS_COMB_MIN_DELAY_MS 0.0f
#define XTS_COMB_MAX_DELAY_MS 5.0f

#define XTS_STATE_VAR_MIN_FREQ_HZ 20.0f
#define XTS_STATE_VAR_MAX_FREQ_HZ 10000.0f

namespace Xts {

static constexpr int COMB_DELAY_MAX_SAMPLES = 
static_cast<int>(XTS_COMB_MAX_DELAY_MS * XTS_MAX_SAMPLE_RATE / 1000.0f + 1.0f);

struct StateVarState
{
  DoubleSample ic1eq;
  DoubleSample ic2eq;
}; 

struct CombState
{
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> x;
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> y;
};

class FilterDSP
{
  float _rate;
  CombState _comb;
  StateVarState _stateVar;
  struct FilterModel const* _model;
public:
  FilterDSP() = default;
  FilterDSP(struct FilterModel const* model, float rate);
public:
  FloatSample GenerateStateVar(FloatSample x, float freq, double res);
  FloatSample GenerateComb(FloatSample x, int minDelay, int plusDelay, float minGain, float plusGain);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP