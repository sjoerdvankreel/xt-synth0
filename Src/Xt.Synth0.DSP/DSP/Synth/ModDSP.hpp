#ifndef XTS_DSP_SYNTH_MOD_DSP_HPP
#define XTS_DSP_SYNTH_MOD_DSP_HPP

#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <DSP/Shared/Modulate.hpp>
#include <Model/Synth/ModModel.hpp>
#include <cstdint>

namespace Xts {

class ModDSP
{
  float _amount;
  CvSample _output;
  ModModel const* _model;
public:
  ModDSP() = default;
  ModDSP(ModModel const* model): _model(model) {}
public:
  CvSample Next(struct CvState const& cv);
  float Amount() const { return _amount; }
  CvSample Output() const { return _output; }
public:
  float Modulate(CvSample carrier) const 
  { return Xts::Modulate(carrier, Output(), Amount()); }
  float Modulate(CvSample carrier, int target) const
  { return _model->target == target ? Modulate(carrier) : carrier.value; }
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_MOD_DSP_HPP