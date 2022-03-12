#ifndef XTS_MODEL_SHARED_MOD_MODEL_HPP
#define XTS_MODEL_SHARED_MOD_MODEL_HPP

#include <Model/Shared/Model.hpp>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN ModModel
{
  int32_t source;
  int32_t amount;
  int32_t target;
  int32_t pad__;

  template <class Target>
  bool IsTarget(Target tgt) const;
};
XTS_CHECK_SIZE(ModModel, 16);

template <class Target>
inline bool
ModModel::IsTarget(Target tgt) const
{ return target == static_cast<int32_t>(tgt); }

} // namespace Xts
#endif // XTS_MODEL_SHARED_MOD_MODEL_HPP