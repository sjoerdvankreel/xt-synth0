#ifndef XTS_MODEL_SYNTH_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_FILTER_MODEL_HPP

#include <Model/Synth/ModsModel.hpp>
#include <Model/Synth/SynthConfig.hpp>

namespace Xts {

enum class FilterType { Biquad, Comb };
enum class BiquadType { LPF, HPF, BPF, BSF };

enum class FilterModTarget
{ 
  BiquadFrequency,
  BiquadResonance,
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

  BiquadType biquadType;
  int32_t biquadResonance;
  int32_t biquadFrequency;
  int32_t pad__;

  ModsModel<FilterModTarget> mods;
  int32_t unitAmount[XTS_SYNTH_UNIT_COUNT];
  int32_t filterAmount[XTS_SYNTH_FILTER_COUNT];
};
XTS_CHECK_SIZE(FilterModel, 96);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_FILTER_MODEL_HPP