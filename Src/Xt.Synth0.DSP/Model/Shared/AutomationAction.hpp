#ifndef XTS_MODEL_SHARED_AUTOMATION_ACTION_HPP
#define XTS_MODEL_SHARED_AUTOMATION_ACTION_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN AutomationAction
{
  int32_t automationId;
  int32_t paramIndex;
  int32_t paramValue;
  int32_t pad__;
};
XTS_CHECK_SIZE(AutomationAction, 16);

} // namespace Xts
#endif // XTS_MODEL_SHARED_AUTOMATION_ACTION_HPP