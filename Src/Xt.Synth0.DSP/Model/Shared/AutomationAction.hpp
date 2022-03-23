#ifndef XTS_MODEL_SHARED_AUTOMATION_ACTION_HPP
#define XTS_MODEL_SHARED_AUTOMATION_ACTION_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN AutomationAction
{
  int32_t target;
  int32_t value;
};
XTS_CHECK_SIZE(AutomationAction, 8);

} // namespace Xts
#endif // XTS_MODEL_SHARED_AUTOMATION_ACTION_HPP