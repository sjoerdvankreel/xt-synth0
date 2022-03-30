#ifndef XTS_MODEL_SYNTH_SYNTH_MODEL_HPP
#define XTS_MODEL_SYNTH_SYNTH_MODEL_HPP

#include <Model/Shared/ParamInfo.hpp>
#include <Model/Synth/VoiceModel.hpp>
#include <Model/Synth/GlobalModel.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN SynthModel
{
  VoiceModel voice;
  GlobalModel global;
public:
  static ParamInfo const* Params();
  static void Init(ParamInfo* params, size_t count);
};
XTS_CHECK_SIZE(SynthModel, 920);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_SYNTH_MODEL_HPP