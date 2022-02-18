#ifndef XTS_SOURCE_DSP_HPP
#define XTS_SOURCE_DSP_HPP

#include "LfoDSP.hpp"
#include "EnvDSP.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

class SourceDSP:
public DSPBase<SourceModel, AudioInput, SourceDSP const*>
{
  LfoDSP _lfos[LfoCount];
  EnvDSP _envs[EnvCount];
public:
  void Next();
  void Release();
  LfoDSP const* Lfos() const { return _lfos; }
  EnvDSP const* Envs() const { return _envs; }
  float Velo() const { return _input->key.amp; }
public:
  SourceDSP() = default;
  SourceDSP const* Value() const { return this; }
  SourceDSP(SourceModel const* model, AudioInput const* input);
};
static_assert(ReleaseableDSP<SourceDSP, SourceModel, AudioInput, SourceDSP const*>);

} // namespace Xts
#endif // XTS_SOURCE_DSP_HPP