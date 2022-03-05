#ifndef XTS_DSP_SYNTH_MOD_DSP_HPP
#define XTS_DSP_SYNTH_MOD_DSP_HPP

#include <DSP/Param.hpp>
#include <DSP/Synth/CvSample.hpp>
#include <Model/Synth/ModModel.hpp>
#include <cstdint>

namespace Xts {

class ModDSP
{
  float _amount;
  ModSource _source;
public:
  ModDSP() = default;
  template <class Target>
  ModDSP(ModModel<Target> const& model);
  ModDSP(ModSource source, int32_t amount);
public:
  float Modulate(CvSample sample, struct CvState const& cv);
};

inline ModDSP::
ModDSP(ModSource source, int32_t amount) :
_source(source),
_amount(Param::Mix(amount)) {}

template <class Target>
inline ModDSP::
ModDSP(ModModel<Target> const& model):
ModDSP(model.source, model.amount) {}

} // namespace Xts
#endif // XTS_DSP_SYNTH_MOD_DSP_HPP