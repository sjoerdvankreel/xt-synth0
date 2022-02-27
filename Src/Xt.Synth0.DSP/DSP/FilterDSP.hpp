#ifndef XTS_FILTER_DSP_HPP
#define XTS_FILTER_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

#define XTS_MAX_COMB_DELAY 256

namespace Xts {

struct BiquadState
{
  double a[3];
  double b[3];
  DAudioOutput x[3];
  DAudioOutput y[3];
};

struct CombState
{
  int delayMin;
  int delayPlus;
  float gainMin;
  float gainPlus;
  FAudioOutput x[XTS_MAX_COMB_DELAY];
  FAudioOutput y[XTS_MAX_COMB_DELAY];
};

union FilterState
{
  CombState comb;
  BiquadState biquad;
};

class FilterDSP
{
  int _index;
  FAudioOutput _output;
  float _amt1, _amt2;
  float _units[UnitCount];
  float _flts[FilterCount];
  FilterModel const* _model;
  FilterState _state;
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, int index, float rate);
public:
  FAudioOutput Output() const { return _output; };
  FAudioOutput Next(CvState const& cv, AudioState const& audio);
  static void Plot(FilterModel const& model, CvModel const& cvModel, AudioModel const& AudioModel, bool spec, int index, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_FILTER_DSP_HPP