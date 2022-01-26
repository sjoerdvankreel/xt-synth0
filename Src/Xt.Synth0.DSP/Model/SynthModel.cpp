#include "SynthModel.hpp"
#include <vector>

namespace Xts {
 
static std::vector<SyncStep> Steps;
SyncStep* SyncSteps() { return Steps.data(); }
void SynthModelInit(SyncStep* steps, int32_t count) 
{ Steps.insert(Steps.begin(), steps, steps + static_cast<size_t>(count)); }

} // namespace Xts