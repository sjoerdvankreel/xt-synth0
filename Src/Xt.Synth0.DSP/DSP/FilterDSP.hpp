#ifndef XTS_FILTER_DSP_HPP
#define XTS_FILTER_DSP_HPP

#include "CvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class FilterDSP
{
  int _index;
  float _amt1, _amt2;
  float _a[3], _b[3];
  AudioOutput _x[3], _y[3];
  float _units[UnitCount];
  float _flts[FilterCount];
  FilterModel const* _model;
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, int index, float rate);
public:
  AudioOutput Output() const { return _y[0]; };
  AudioOutput Next(CvState const& cv, AudioState const& audio);
};

} // namespace Xts
#endif // XTS_FILTER_DSP_HPP