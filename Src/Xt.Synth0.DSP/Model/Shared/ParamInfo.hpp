#ifndef XTS_MODEL_SHARED_PARAM_INFO_HPP
#define XTS_MODEL_SHARED_PARAM_INFO_HPP

#include <Model/Shared/Model.hpp>

namespace xts {

struct XTS_ALIGN ParamInfo 
{
  int32_t min;
  int32_t max; 
};
XTS_CHECK_SIZE(ParamInfo, 8);

} // namespace Xts
#endif // XTS_MODEL_SHARED_PARAM_INFO_HPP