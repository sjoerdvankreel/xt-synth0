#ifndef XTS_MODEL_SYNTH_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_MOD_MODEL_HPP

#include <Model/Model.hpp>
#include <cstdint>

namespace Xts {

enum class ModSource
{
  Velocity, 
  Env1, Env2, Env3, 
  LFO1, LFO2, LFO3 
};

template <class Target>
struct XTS_ALIGN ModModel
{
  Target target;
  int32_t amount;
  ModSource source;
  int32_t pad__;
};
typedef ModModel<int32_t> AnyModModel;
XTS_CHECK_SIZE(AnyModModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_MOD_MODEL_HPP