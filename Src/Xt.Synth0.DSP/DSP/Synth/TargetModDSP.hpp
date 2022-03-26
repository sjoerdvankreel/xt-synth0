#ifndef XTS_DSP_SYNTH_TARGET_MOD_DSP_HPP
#define XTS_DSP_SYNTH_TARGET_MOD_DSP_HPP

#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/Modulate.hpp>
#include <Model/Synth/TargetModModel.hpp>
#include <cstdint>

namespace Xts {

class TargetModDSP
{
  float _amount;
  CvSample _output;
  TargetModModel const* _model;
public:
  TargetModDSP() = default;
  TargetModDSP(TargetModModel const* model): _model(model) {}
public:
  CvSample Next(struct CvState const& cv);
  float Amount() const { return _amount; }
  CvSample Output() const { return _output; }
public:
  float Modulate(CvSample carrier, int target) const
  { return _model->target == target ? Xts::Modulate(carrier, Output(), Amount()) : carrier.value; }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_TARGET_MOD_DSP_HPP