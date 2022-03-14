#include <Model/Synth/SynthModel.hpp>

#include <vector>
#include <cassert>

namespace Xts {
 
static std::vector<ParamInfo> _params;

ParamInfo const* 
SynthModel::Params() 
{ return _params.data(); }

void 
SynthModel::Init(ParamInfo* params, size_t count)
{ _params.insert(_params.begin(), params, params + count); }

} // namespace Xts