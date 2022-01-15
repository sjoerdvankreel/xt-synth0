#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/SynthModel.hpp"

namespace Xts {

enum class EnvStage { Dly, A, Hld, D, S, R, End };

struct EnvOutput
{
  float lvl;
  bool staged;
};

struct EnvParams
{
  float dly, hld;
  float a, d, s, r;
};

class EnvDSP 
{
  int _stagePos = 0;
  EnvStage _stage = EnvStage::Dly; 
  EnvStage _prevStage = EnvStage::Dly;

public:
  void Init();
  void Release();
  EnvParams Params(EnvModel const& env, float rate) const;
  void Next(EnvModel const& env, float rate, EnvOutput& output);

private:
  void NextStage(EnvStage stage);
  void CycleStage(EnvParams const& params);
  float Generate(float from, float to, float len, int slp) const;
  float Generate(EnvModel const& env, EnvParams const& params) const;
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP