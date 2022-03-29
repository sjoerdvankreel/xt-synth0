#ifndef XTS_DSP_SYNTH_GLOBAL_FILTER_DSP_HPP
#define XTS_DSP_SYNTH_GLOBAL_FILTER_DSP_HPP

#include <DSP/Synth/FilterDSP.hpp>
#include <DSP/Synth/GlobalModDSP.hpp>

namespace Xts {

class GlobalFilterDSP
{
  FilterDSP _filter;
  GlobalModDSP _mod;
  FloatSample _output;
  struct GlobalFilterModel const* _model;
private:
  FloatSample GenerateComb(CvSample globalLfo);
  FloatSample GenerateStateVar(CvSample globalLfo);
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(CvSample globalLfo, FloatSample x);
public:
  GlobalFilterDSP() = default;
  GlobalFilterDSP(struct GlobalFilterModel const* model, float rate);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_GLOBAL_FILTER_DSP_HPP