#ifndef XTS_MODEL_SYNTH_TARGET_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_TARGET_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <Model/Synth/ModModel.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN TargetModModel
{
  ModModel mod;
  int32_t target;
  int32_t pad__;
};
XTS_CHECK_SIZE(TargetModModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_TARGET_MOD_MODEL_HPP