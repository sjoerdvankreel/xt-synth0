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
  int32_t const* _amountModel;
public:
  float Modulate(CvSample carrier) const
  { return Xts::Modulate(carrier, Output(), Amount()); }
public:
  ModDSP() = default;
  ModDSP(int32_t const* amountModel) : _amountModel(amountModel) {}
public:
  float Amount() const { return _amount; }
  CvSample Output() const { return _output; }
  CvSample Next(ModSource source, struct CvState const& cv);
};

} // namespace Xts
#endif // XTS_DSP_SYNTH_MOD_DSP_HPP