#ifndef XTS_MODEL_SHARED_PARAM_BINDING_HPP
#define XTS_MODEL_SHARED_PARAM_BINDING_HPP

#include <Model/Model.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN ParamBinding
{
  int32_t** params; 
};
XTS_CHECK_SIZE(ParamBinding, 8);

} // namespace Xts
#endif // XTS_MODEL_SHARED_PARAM_BINDING_HPP