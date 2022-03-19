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
  ModSource _source;
public:
  ModDSP() = default;
  template <class Target>
  ModDSP(ModModel<Target> const& model);
  ModDSP(ModSource source, int32_t amount);
public:
  float Amount() const { return _amount; };
  CvSample Output() const { return _output; }
public:
  CvSample Next(struct CvState const& cv);
  float Modulate(CvSample carrier) const 
  { return Xts::Modulate(carrier, Output(), Amount()); }
  template <class Target>
  float Modulate(Target model, Target target, CvSample carrier) const 
  { return model == target ? Modulate(carrier) : carrier.value; }
};

inline ModDSP::
ModDSP(ModSource source, int32_t amount) :
_output(),
_source(source),
_amount(Param::Mix(amount)) {}

template <class Target>
inline ModDSP::
ModDSP(ModModel<Target> const& model):
ModDSP(model.source, model.amount) {}

} // namespace Xts
#endif // XTS_DSP_SYNTH_MOD_DSP_HPP