#ifndef XTS_MODEL_SYNTH_VOICE_FILTER_MODEL_HPP
#define XTS_MODEL_SYNTH_VOICE_FILTER_MODEL_HPP

#include <Model/Synth/SynthConfig.hpp>
#include <Model/Synth/FilterModel.hpp>
#include <Model/Synth/TargetModsModel.hpp>

namespace Xts {

struct XTS_ALIGN VoiceFilterModel
{
  FilterModel filter;
  TargetModsModel mods;
  int32_t unitAmount[XTS_VOICE_UNIT_COUNT];
  int32_t filterAmount[XTS_VOICE_FILTER_COUNT];
  int32_t pad__;
};
XTS_CHECK_SIZE(VoiceFilterModel, 96);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_VOICE_FILTER_MODEL_HPP