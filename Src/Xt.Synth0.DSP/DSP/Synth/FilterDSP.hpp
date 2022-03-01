#ifndef XTS_DSP_SYNTH_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_FILTER_DSP_HPP

#include <DSP/Param.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <Model/DSPModel.hpp>
#include <Model/SynthModel.hpp>
#include <DSP/AudioSample.hpp>

#define XTS_COMB_MIN_DELAY_MS 0.0f
#define XTS_COMB_MAX_DELAY_MS 10.0f

namespace Xts {

static constexpr int COMB_DELAY_MAX_SAMPLES 
= static_cast<int>(XTS_COMB_MAX_DELAY_MS * XTS_MAX_SAMPLE_RATE / 1000.0f);

struct BiquadState
{
  double a[3];
  double b[3];
  DoubleSample x[3];
  DoubleSample y[3];
};

struct CombState
{
  int minDelay;
  int plusDelay;
  float minGain;
  float plusGain;
  FloatSample x[COMB_DELAY_MAX_SAMPLES];
  FloatSample y[COMB_DELAY_MAX_SAMPLES];
};

union FilterState
{
  CombState comb;
  BiquadState biquad;
};

class FilterDSP
{
  int _index;
  float _modAmount1;
  float _modAmount2;
  FilterState _state;
  FloatSample _output;
  FilterModel const* _model;
  float _unitAmount[UnitCount];
  float _filterAmount[FilterCount];
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, int index, float rate);
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(CvState const& cv, AudioState const& audio);
  static void Plot(FilterModel const& model, CvModel const& cvModel, AudioModel const& AudioModel, bool spec, int index, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_DSP_HPP