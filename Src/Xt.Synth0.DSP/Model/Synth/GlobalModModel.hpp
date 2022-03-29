#ifndef XTS_MODEL_SYNTH_GLOBAL_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_GLOBAL_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN GlobalModModel
{
  int32_t amount;
  int32_t target;
};
XTS_CHECK_SIZE(GlobalModModel, 8);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_GLOBAL_MOD_MODEL_HPP