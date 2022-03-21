#ifndef XTS_MODEL_SYNTH_SYNTH_MODEL_HPP
#define XTS_MODEL_SYNTH_SYNTH_MODEL_HPP

#include <Model/Synth/VoiceModel.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN SynthModel
{
  VoiceModel voice;
public:
  static ParamInfo const* Params();
  static void Init(ParamInfo* params, size_t count);
};
XTS_CHECK_SIZE(SynthModel, 928);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_SYNTH_MODEL_HPP