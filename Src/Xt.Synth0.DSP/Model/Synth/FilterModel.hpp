#ifndef XTS_MODEL_SYNTH_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_FILTER_MODEL_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

enum class FilterType { Ladder, StateVar, Comb };
enum class StateVarPassType { LPF, HPF, BPF, BSF };

enum class FilterModTarget
{
  LPHP,
  Drive,
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

  int32_t resonance;
  int32_t frequency;
  int32_t ladderLpHp;
  int32_t ladderDrive;
  StateVarPassType stateVarPassType;
  int32_t pad__;
};
XTS_CHECK_SIZE(FilterModel, 48);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_FILTER_MODEL_HPP