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
public:
  ModsDSP() = default;
  ModsDSP(ModsModel const* model):
  _mod1(&model->mod1), _mod2(&model->mod2) {}
public:
  ModDSP const& Mod1() const { return _mod1; }
  ModDSP const& Mod2() const { return _mod2; }
public:
  void Next(struct CvState const& cv);
  float Modulate(CvSample carrier, int target) const;
};

inline void
ModsDSP::Next(struct CvState const& cv)
{
  _mod1.Next(cv);
  _mod2.Next(cv);
}

inline float 
ModsDSP::Modulate(CvSample carrier, int target) const
{
  carrier.value = _mod1.Modulate(carrier, target);
  carrier.value = _mod2.Modulate(carrier, target);
  return carrier.value;
}

} // namespace Xts
#endif // XTS_DSP_SYNTH_MODS_DSP_HPP