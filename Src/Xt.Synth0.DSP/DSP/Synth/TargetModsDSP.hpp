#ifndef XTS_DSP_SYNTH_TARGET_MODS_DSP_HPP
#define XTS_DSP_SYNTH_TARGET_MODS_DSP_HPP

#include <DSP/Shared/CvSample.hpp>
#include <DSP/Synth/TargetModDSP.hpp>
#include <Model/Synth/TargetModsModel.hpp>
#include <cstdint>

namespace Xts {

class TargetModsDSP
{
  TargetModDSP _mod1;
  TargetModDSP _mod2;
public:
  TargetModsDSP() = default;
  TargetModsDSP(TargetModsModel const* model):
  _mod1(&model->mod1), _mod2(&model->mod2) {}
public:
  TargetModDSP const& Mod1() const { return _mod1; }
  TargetModDSP const& Mod2() const { return _mod2; }
public:
  void Next(struct CvState const& cv);
  float Modulate(CvSample carrier, int target) const;
};

inline void
TargetModsDSP::Next(struct CvState const& cv)
{
  _mod1.Next(cv);
  _mod2.Next(cv);
}

inline float 
TargetModsDSP::Modulate(CvSample carrier, int target) const
{
  carrier.value = _mod1.Modulate(carrier, target);
  carrier.value = _mod2.Modulate(carrier, target);
  return carrier.value;
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_TARGET_MODS_DSP_HPP