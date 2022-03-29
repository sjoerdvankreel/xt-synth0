#ifndef XTS_MODEL_SYNTH_TARGET_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_TARGET_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN TargetModModel
{
  int32_t amount;
  int32_t target;
};
XTS_CHECK_SIZE(TargetModModel, 8);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_TARGET_MOD_MODEL_HPP