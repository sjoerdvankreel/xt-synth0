#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/SynthModel.hpp"

namespace Xts {

enum class EnvStage { Dly, A, Hld, D, S, R, End };

class EnvDSP 
{
  int _stagePos = 0;
  EnvStage _stage = EnvStage::Dly;
public:
  void Reset();
  void Length(
    EnvModel const& env, float rate, float* dly,
    float* a, float* hld, float* d, float* r) const;
  float Next(EnvModel const& env, float rate, bool active, EnvStage* stage);
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP