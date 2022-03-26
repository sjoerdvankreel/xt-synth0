#ifndef XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP

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
  DoubleSample ic1eq;
  DoubleSample ic2eq;
}; 

struct CombState
{
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> x;
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> y;
};

union FilterState
{
  CombState comb;
  StateVarState stateVar;
};

class VoiceFilterDSP
{
  int _index;
  float _rate;
  FilterState _state;
  FloatSample _output;
  TargetModsDSP _mods;
  struct VoiceFilterModel const* _model;
private:
  FloatSample GenerateComb();
  FloatSample GenerateStateVar();
public:
  VoiceFilterDSP() = default;
  VoiceFilterDSP(struct VoiceFilterModel const* model, int index, float rate);
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_VOICE_FILTER_DSP_HPP