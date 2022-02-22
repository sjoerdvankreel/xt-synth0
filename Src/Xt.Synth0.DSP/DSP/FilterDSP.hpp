#ifndef XTS_FILTER_DSP_HPP
#define XTS_FILTER_DSP_HPP

#include "CvDSP.hpp"
#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class FilterDSP
{
  float _a[2], _b[3];
  AudioOutput _output;
  AudioOutput _x[2], _y[2];
  FilterModel const* _model;
  float _units[UnitCount];
  float _flts[FilterCount];
  float _rate, _amt1, _amt2;
public:
  FilterDSP() = default;
  FilterDSP(FilterModel const* model, float rate);
public:
  AudioOutput Output() const { return _output; };
  AudioOutput Next(CvState const& cv, AudioState const& audio);
};

} // namespace Xts
#endif // XTS_FILTER_DSP_HPP