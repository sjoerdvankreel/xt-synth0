#ifndef XTS_DSP_SYNTH_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_FILTER_DSP_HPP

#include <DSP/Synth/ModsDSP.hpp>
#include <DSP/Shared/Config.hpp>
#include <DSP/Shared/DelayBuffer.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <Model/Synth/SynthConfig.hpp>

#define XTS_COMB_MIN_DELAY_MS 0.0f
#define XTS_COMB_MAX_DELAY_MS 5.0f

namespace Xts {

static constexpr int COMB_DELAY_MAX_SAMPLES = 
static_cast<int>(XTS_COMB_MAX_DELAY_MS * XTS_MAX_SAMPLE_RATE / 1000.0f + 1.0f);

struct BiquadState
{
  double a[3];
  double b[3];
  DelayBuffer<DoubleSample, 3> x;
  DelayBuffer<DoubleSample, 3> y;
};

struct StateVarState
{
  DelayBuffer<DoubleSample, 2> v;
  DelayBuffer<DoubleSample, 2> iceq;
};

struct CombState
{
  float minGain;
  float plusGain;
  float minDelay;
  float plusDelay;
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> x;
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> y;
};

union FilterState
{
  CombState comb;
  BiquadState biquad;
  StateVarState stateVar;
};

class FilterDSP
{
  int _index;
  float _rate;
  ModsDSP _mods;
  FilterState _state;
  FloatSample _output;
  struct FilterModel const* _model;
  float _unitAmount[XTS_SYNTH_UNIT_COUNT];
  float _filterAmount[XTS_SYNTH_FILTER_COUNT];
private:
  FloatSample GenerateComb();
  FloatSample GenerateBiquad();
  FloatSample GenerateStateVar();
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, int index, float rate);
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_DSP_HPP