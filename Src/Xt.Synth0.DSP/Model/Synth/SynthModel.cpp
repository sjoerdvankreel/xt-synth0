#include <Model/Synth/SynthModel.hpp>
#include <vector>
#include <cassert>

namespace Xts {
 
static std::vector<ParamInfo> Infos;

ParamInfo* ParamInfos() { return Infos.data(); }

void
SynthModelInit(
  struct ParamInfo* infos, int32_t infoCount)
{
  Infos.insert(Infos.begin(), infos, infos + static_cast<size_t>(infoCount));
}

} // namespace Xts