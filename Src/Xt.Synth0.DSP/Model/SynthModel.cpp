#include "SynthModel.hpp"
#include <vector>
#include <cassert>

namespace Xts {
 
static std::vector<SyncStep> Steps;
static std::vector<ParamInfo> Infos;

SyncStep* SyncSteps() { return Steps.data(); }
ParamInfo* ParamInfos() { return Infos.data(); }

void
SynthModelInit(
  struct ParamInfo* infos, int32_t infoCount,
  struct SyncStep* steps, int32_t stepCount)
{
  Steps.insert(Steps.begin(), steps, steps + static_cast<size_t>(stepCount));
  Infos.insert(Infos.begin(), infos, infos + static_cast<size_t>(infoCount));
}

} // namespace Xts