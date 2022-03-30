#ifndef XTS_MODEL_SYNTH_AMP_MODEL_HPP
#define XTS_MODEL_SYNTH_AMP_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/VoiceModModel.hpp>

namespace Xts {

struct XTS_ALIGN AmpModel
{
  int32_t amp;
  int32_t pan;
  VoiceModModel ampMod;
  VoiceModModel panMod;
  int32_t unitAmount[XTS_VOICE_UNIT_COUNT];
  int32_t filterAmount[XTS_VOICE_FILTER_COUNT];
  int32_t pad__;
};
XTS_CHECK_SIZE(AmpModel, 48);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_AMP_MODEL_HPP