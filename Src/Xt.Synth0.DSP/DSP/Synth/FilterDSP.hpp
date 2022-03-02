#ifndef XTS_DSP_SYNTH_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_FILTER_DSP_HPP

#include <DSP/Param.hpp>
#include <DSP/DelayBuffer.hpp>
#include <DSP/Synth/CvState.hpp>
#include <DSP/Synth/AudioState.hpp>
#include <Model/DSPModel.hpp>
#include <Model/SynthModel.hpp>
#include <DSP/AudioSample.hpp>

#define XTS_COMB_MIN_DELAY_MS 0.0f
#define XTS_COMB_MAX_DELAY_MS 5.0f

namespace Xts {

static constexpr int COMB_DELAY_MAX_SAMPLES = 
static_cast<int>(XTS_COMB_MAX_DELAY_MS * XTS_MAX_SAMPLE_RATE / 1000.0f + 1.0f);

struct FilterPlotState
{
  int index;
  bool spectrum;
  PlotOutput* output;
  PlotInput const* input;
  CvModel const* cvModel;
  FilterModel const* model;
  AudioModel const* audioModel;
};

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
  static void Plot(FilterPlotState* state);
  FloatSample Output() const { return _output; };
  FloatSample Next(CvState const& cv, AudioState const& audio);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_FILTER_DSP_HPP