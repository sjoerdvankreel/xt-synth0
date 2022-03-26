#ifndef XTS_DSP_SYNTH_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_FILTER_DSP_HPP

#include <DSP/Shared/Config.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/DelayBuffer.hpp>
#include <DSP/Shared/AudioSample.hpp>
#include <DSP/Synth/TargetModsDSP.hpp>
#include <Model/Synth/SynthConfig.hpp>

#define XTS_COMB_MIN_DELAY_MS 0.0f
#define XTS_COMB_MAX_DELAY_MS 5.0f

namespace Xts {

static constexpr int COMB_DELAY_MAX_SAMPLES = 
static_cast<int>(XTS_COMB_MAX_DELAY_MS * XTS_MAX_SAMPLE_RATE / 1000.0f + 1.0f);

struct StateVarState
{
  float resonance;
  float frequency;
  DoubleSample ic1eq;
  DoubleSample ic2eq;
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
  StateVarState stateVar;
};

class FilterDSP
{
  int _index;
  float _rate;
  FilterState _state;
  FloatSample _output;
  TargetModsDSP _mods;
  struct FilterModel const* _model;
  float _unitAmount[XTS_VOICE_UNIT_COUNT];
  float _filterAmount[XTS_VOICE_FILTER_COUNT];
private:
  FloatSample GenerateComb();
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