#ifndef XTS_DSP_SYNTH_MODS_DSP_HPP
#define XTS_DSP_SYNTH_MODS_DSP_HPP

#include <DSP/Synth/ModDSP.hpp>
#include <DSP/Shared/CvSample.hpp>
#include <Model/Synth/ModsModel.hpp>
#include <cstdint>

namespace Xts {

class ModsDSP
{
  ModDSP _mod1;
  ModDSP _mod2;
  int32_t _target1;
  int32_t _target2;
public:
  ModsDSP() = default;
  template <class Target>
  ModsDSP(ModsModel<Target> const& model);
public:
  ModDSP const& Mod1() const { return _mod1; }
  ModDSP const& Mod2() const { return _mod2; }
public:
  void Next(struct CvState const& cv);
  template <class Target>
  float Modulate(Target target, CvSample carrier) const;
};

inline void
ModsDSP::Next(struct CvState const& cv)
{
  _mod1.Next(cv);
  _mod2.Next(cv);
}

template <class Target>
inline ModsDSP::
ModsDSP(ModsModel<Target> const& model):
_mod1(model.mod1),
_mod2(model.mod2),
_target1(static_cast<int32_t>(model.mod1.target)),
_target2(static_cast<int32_t>(model.mod2.target)) {}

template <class Target>
inline float 
ModsDSP::Modulate(Target target, CvSample carrier) const
{
  int32_t t = static_cast<int32_t>(target);
  carrier.value = _mod1.Modulate(_target1, t, carrier);
  carrier.value = _mod2.Modulate(_target2, t, carrier);
  return carrier.value;
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_MODS_DSP_HPP