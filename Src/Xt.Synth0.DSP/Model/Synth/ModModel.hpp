#ifndef XTS_MODEL_SYNTH_MOD_MODEL_HPP
#define XTS_MODEL_SYNTH_MOD_MODEL_HPP

#include <Model/Model.hpp>

namespace Xts {

enum class ModSource
{
  Velo, 
  Env1, 
  Env2, 
  Env3, 
  LFO1, 
  LFO2, 
  LFO3 
};

struct XTS_ALIGN ModModel
{
  ModModel() = default;
  ModModel(ModModel const&) = delete;

  int32_t amount;
  int32_t target;
  ModSource source;
  int32_t pad__;
};
XTS_CHECK_SIZE(ModModel, 16);

} // namespace Xts
#endif // XTS_MODEL_SYNTH_MOD_MODEL_HPP