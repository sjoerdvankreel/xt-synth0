#ifndef XTS_DSP_SYNTH_GLOBAL_MOD_DSP_HPP
#define XTS_DSP_SYNTH_GLOBAL_MOD_DSP_HPP

#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/Modulate.hpp>
#include <Model/Synth/GlobalModModel.hpp>
#include <cstdint>

namespace Xts {

class GlobalModDSP
{
  GlobalModModel const* _model;
public:
  GlobalModDSP() = default;
  GlobalModDSP(GlobalModModel const* model): _model(model) {}
public:
  float Modulate(CvSample globalLfo, CvSample carrier, int target) const
  { return _model->target == target ? Xts::Modulate(carrier, globalLfo, Param::Mix(_model->amount)) : carrier.value; }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_GLOBAL_MOD_DSP_HPP