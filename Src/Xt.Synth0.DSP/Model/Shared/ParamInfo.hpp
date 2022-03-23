#ifndef XTS_MODEL_SHARED_PARAM_INFO_HPP
#define XTS_MODEL_SHARED_PARAM_INFO_HPP

#include <Model/Shared/Model.hpp>

namespace Xts {

struct XTS_ALIGN ParamInfo 
{
  int32_t min;
  int32_t max; 
  XtsBool realtime;
  int32_t automationId;
};
XTS_CHECK_SIZE(ParamInfo, 16);

} // namespace Xts
#endif // XTS_MODEL_SHARED_PARAM_INFO_HPP