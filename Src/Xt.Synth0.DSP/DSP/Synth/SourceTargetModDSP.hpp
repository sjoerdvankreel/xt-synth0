#ifndef XTS_DSP_SYNTH_SOURCE_TARGET_MOD_DSP_HPP
#define XTS_DSP_SYNTH_SOURCE_TARGET_MOD_DSP_HPP

#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <Model/Synth/SourceTargetModModel.hpp>
#include <cstdint>

namespace Xts {

class SourceTargetModDSP
{
  ModDSP _dsp;
  SourceTargetModModel const* _model;
public:
  SourceTargetModDSP() = default;
  SourceTargetModDSP(SourceTargetModModel const* model):
  _dsp(&model->amount), _model(model) {}
public:
  float Modulate(CvSample carrier, int target) const
  { return _model->target == target ? _dsp.Modulate(carrier) : carrier.value; }
public:
  float Amount() const { return _dsp.Amount(); }
  CvSample Output() const { return _dsp.Output(); }
  CvSample Next(struct CvState const& cv) { return _dsp.Next(_model->source, cv); }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_SOURCE_TARGET_MOD_DSP_HPP