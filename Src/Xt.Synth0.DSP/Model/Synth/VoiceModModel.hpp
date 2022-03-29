#ifndef XTS_MODEL_SYNTH_VOICE_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_VOICE_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

enum class VoiceModSource
{
  Velocity, 
  Env1, Env2, Env3, 
  LFO1, LFO2, GlobalLFO
};

struct XTS_ALIGN VoiceModModel
{
  int32_t amount;
  VoiceModSource source;
};
XTS_CHECK_SIZE(VoiceModModel, 8);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_VOICE_MOD_MODEL_HPP