#ifndef XTS_MODEL_SYNTH_SOURCE_TARGET_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_SOURCE_TARGET_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/SourceModModel.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN SourceTargetModModel
{
  int32_t target;
  int32_t amount;
  ModSource source;
  int32_t pad__;
};
XTS_CHECK_SIZE(SourceTargetModModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_SOURCE_TARGET_MOD_MODEL_HPP