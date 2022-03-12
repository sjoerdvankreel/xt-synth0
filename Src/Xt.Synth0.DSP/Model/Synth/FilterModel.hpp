#ifndef XTS_MODEL_SYNTH_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_FILTER_MODEL_HPP

#include <Model/Synth/Config.hpp>
#include <Model/Shared/ModModel.hpp>

namespace Xts {

enum class FilterType { Biquad, Comb };
enum class BiquadType { LPF, HPF, BPF, BSF };

enum class FilterModTarget
{ 
  CombMinGain,
  CombPlusGain,
  CombMinDelay,
  CombPlusDelay,
  BiquadFrequency,
  BiquadResonance
};

struct XTS_ALIGN FilterModel
{
  XtsBool on;
  FilterType type;

  int32_t combMinGain;
  int32_t combPlusGain;
  int32_t combMinDelay;
  int32_t combPlusDelay;

  BiquadType biquadType;
  int32_t biquadResonance;
  int32_t biquadFrequency;
  int32_t pad__;

  ModModel mod1;
  ModModel mod2;
  int32_t unitAmount[XTS_SYNTH_UNIT_COUNT];
  int32_t filterAmount[XTS_SYNTH_FILTER_COUNT];
};
XTS_CHECK_SIZE(FilterModel, 96);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_FILTER_MODEL_HPP