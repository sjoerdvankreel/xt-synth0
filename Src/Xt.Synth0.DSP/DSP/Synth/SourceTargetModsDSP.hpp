#ifndef XTS_DSP_SYNTH_SOURCE_TARGET_MODS_DSP_HPP
#define XTS_DSP_SYNTH_SOURCE_TARGET_MODS_DSP_HPP

#include <DSP/Shared/CvSample.hpp>
#include <DSP/Synth/SourceTargetModDSP.hpp>
#include <Model/Synth/SourceTargetModsModel.hpp>
#include <cstdint>

namespace Xts {

class SourceTargetModsDSP
{
  SourceTargetModDSP _mod1;
  SourceTargetModDSP _mod2;
public:
  void Next(struct CvState const& cv);
  float Modulate(CvSample carrier, int target) const;
public:
  SourceTargetModsDSP() = default;
  SourceTargetModsDSP(SourceTargetModsModel const* model):
  _mod1(&model->mod1), _mod2(&model->mod2) {}
public:
  SourceTargetModDSP const& Mod1() const { return _mod1; }
  SourceTargetModDSP const& Mod2() const { return _mod2; }
};

inline void
SourceTargetModsDSP::Next(struct CvState const& cv)
{
  _mod1.Next(cv);
  _mod2.Next(cv);
}

inline float 
SourceTargetModsDSP::Modulate(CvSample carrier, int target) const
{
  carrier.value = _mod1.Modulate(carrier, target);
  carrier.value = _mod2.Modulate(carrier, target);
  return carrier.value;
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_SOURCE_TARGET_MODS_DSP_HPP