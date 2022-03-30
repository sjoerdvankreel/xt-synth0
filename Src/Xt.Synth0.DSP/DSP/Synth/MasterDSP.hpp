#ifndef XTS_DSP_SYNTH_MASTER_DSP_HPP
#define XTS_DSP_SYNTH_MASTER_DSP_HPP

#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/AudioSample.hpp>

namespace Xts {

class MasterDSP
{
  FloatSample _output;
  struct MasterModel const* _model;
public:
  FloatSample Output() const { return _output; };
  FloatSample Next(CvSample globalLfo, FloatSample x);
public:
  MasterDSP() = default;
  MasterDSP(struct MasterModel const* model): _model(model) {}
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_MASTER_DSP_HPP