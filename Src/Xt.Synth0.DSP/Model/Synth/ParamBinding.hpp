#ifndef XTS_MODEL_SYNTH_PARAM_BINDING_HPP
#define XTS_MODEL_SYNTH_PARAM_BINDING_HPP

#include <Model/Model.hpp>
#include <Model/Synth/Config.hpp>

#include <cstdint>

namespace Xts {

struct XTS_ALIGN ParamBinding
{
  int32_t* params[XTS_SYNTH_PARAM_COUNT]; 
};
XTS_CHECK_SIZE(ParamBinding, 1672);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_PARAM_BINDING_HPP