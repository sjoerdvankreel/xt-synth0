#ifndef XTS_DSP_SYNTH_TARGET_MOD_DSP_HPP
#define XTS_DSP_SYNTH_TARGET_MOD_DSP_HPP

#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <Model/Synth/TargetModModel.hpp>
#include <cstdint>

namespace Xts {

class TargetModDSP
{
  ModDSP _dsp;
  TargetModModel const* _model;
public:
  TargetModDSP() = default;
  TargetModDSP(TargetModModel const* model): 
  _dsp(&model->mod), _model(model) {}
public:
  float Amount() const { return _dsp.Amount(); }
  CvSample Output() const { return _dsp.Output(); }
  CvSample Next(struct CvState const& cv) { return _dsp.Next(cv); }
public:
  float Modulate(CvSample carrier, int target) const
  { return _model->target == target ? _dsp.Modulate(carrier) : carrier.value; }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_TARGET_MOD_DSP_HPP