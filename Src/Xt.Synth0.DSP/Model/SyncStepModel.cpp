#include <Model/SyncStepModel.hpp>

namespace Xts {

std::vector<SyncStepModel>
SyncStepModel::_steps;

void
SyncStepModel::Init(SyncStepModel const* steps, size_t count)
{ _steps.insert(_steps.begin(), steps, steps + count); }

} // namespace Xts