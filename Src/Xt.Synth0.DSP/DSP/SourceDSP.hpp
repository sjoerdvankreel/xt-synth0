#ifndef XTS_SOURCE_DSP_HPP
#define XTS_SOURCE_DSP_HPP

#include "LfoDSP.hpp"
#include "EnvDSP.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SourceDSP:
public DSPBase<SourceModel, SourceInput, SourceDSP*>
{
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvCount];
public:
  void Next();
  void Release();
  LfoDSP const* Lfos() const { return _lfos; }
  EnvDSP const* Envs() const { return _envs; }
public:
  SourceDSP() = default;
  SourceDSP(SourceModel const* model, SourceInput const* input);
};
static_assert(ReleaseableDSP<SourceDSP, SourceModel, SourceInput, SourceDSP*>);

} // namespace Xts
#endif // XTS_SOURCE_DSP_HPP