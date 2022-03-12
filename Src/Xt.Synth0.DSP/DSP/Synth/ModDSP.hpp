#ifndef XTS_DSP_SYNTH_MOD_DSP_HPP
#define XTS_DSP_SYNTH_MOD_DSP_HPP

#include <DSP/Shared/Param.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <Model/Shared/ModModel.hpp>

#include <cstdint>

namespace Xts {

class ModDSP
{
  float _amount;
  int32_t _source;
public:
  float Amount() const { return _amount; };
  CvSample Modulator(struct CvState const& cv) const;
  float Modulate(CvSample carrier, CvSample modulator) const;
public:
  ModDSP() = default;
  ModDSP(ModModel const& model);
  template <class Source> ModDSP(Source source, int32_t amount);
};

template <class Source>
inline ModDSP::
ModDSP(Source source, int32_t amount) :
_source(static_cast<int32_t>(source)),
_amount(Param::Mix(amount)) {}

inline ModDSP::
ModDSP(ModModel const& model):
ModDSP(model.source, model.amount) {}

} // namespace Xts
#endif // XTS_DSP_SYNTH_MOD_DSP_HPP