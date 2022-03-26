#ifndef XTS_MODEL_SYNTH_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_FILTER_MODEL_HPP

#include <Model/Synth/SynthConfig.hpp>
#include <Model/Synth/TargetModsModel.hpp>

namespace Xts {

enum class FilterType { StateVar, Comb };
enum class PassType { LPF, HPF, BPF, BSF };

enum class FilterModTarget
{ 
  Frequency,
  Resonance,
  CombMinGain,
  CombPlusGain,
  CombMinDelay,
  CombPlusDelay
};

struct XTS_ALIGN FilterModel
{
  XtsBool on;
  FilterType type;

  int32_t combMinGain;
  int32_t combPlusGain;
  int32_t combMinDelay;
  int32_t combPlusDelay;

  PassType passType;
  int32_t resonance;
  int32_t frequency;
  int32_t pad__;

  TargetModsModel mods;
  int32_t unitAmount[XTS_VOICE_UNIT_COUNT];
  int32_t filterAmount[XTS_VOICE_FILTER_COUNT];
};
XTS_CHECK_SIZE(FilterModel, 96);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_FILTER_MODEL_HPP