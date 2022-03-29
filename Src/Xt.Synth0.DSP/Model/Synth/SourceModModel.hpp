#ifndef XTS_MODEL_SYNTH_SOURCE_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_SOURCE_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

enum class ModSource
{
  Velocity, 
  Env1, Env2, Env3, 
  LFO1, LFO2, GlobalLFO
};

struct XTS_ALIGN SourceModModel
{
  int32_t amount;
  ModSource source;
};
XTS_CHECK_SIZE(SourceModModel, 8);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_SOURCE_MOD_MODEL_HPP