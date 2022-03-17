#ifndef XTS_DSP_SYNTH_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_FILTER_DSP_HPP

#include <DSP/Synth/ModDSP.hpp>
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

struct CombState
{
  int minDelay;
  int plusDelay;
  float minGain;
  float plusGain;
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> x;
  DelayBuffer<FloatSample, COMB_DELAY_MAX_SAMPLES> y;
};

union FilterState
{
  CombState comb;
  BiquadState biquad;
};

class FilterDSP
{
  int _index;
  ModDSP _mod1;
  ModDSP _mod2;
  FilterState _state;
  FloatSample _output;
  struct FilterModel const* _model;
  float _unitAmount[XTS_SYNTH_UNIT_COUNT];
  float _filterAmount[XTS_SYNTH_FILTER_COUNT];
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, int index, float rate);
private:
  FloatSample GenerateComb(CvSample modulator1, CvSample modulator2);
  FloatSample GenerateBiquad(CvSample modulator1, CvSample modulator2);
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(struct CvState const& cv, struct AudioState const& audio);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_DSP_HPP