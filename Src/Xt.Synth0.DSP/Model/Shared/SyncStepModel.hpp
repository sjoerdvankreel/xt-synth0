#ifndef XTS_MODEL_SHARED_SYNC_STEP_MODEL_HPP
#define XTS_MODEL_SHARED_SYNC_STEP_MODEL_HPP

#include <Model/Model.hpp>
#include <vector>
#include <cstdint>

namespace Xts {

struct XTS_ALIGN SyncStepModel
{ 
  int32_t numerator;
  int32_t denominator; 
  static SyncStepModel const* Steps();
  static void Init(SyncStepModel const* steps, size_t count);
private:
  static std::vector<SyncStepModel> _steps;
};
XTS_CHECK_SIZE(SyncStepModel, 8);

inline SyncStepModel const*
SyncStepModel::Steps()
{ return _steps.data(); }

} // namespace Xts
#endif // XTS_MODEL_SHARED_SYNC_STEP_MODEL_HPP