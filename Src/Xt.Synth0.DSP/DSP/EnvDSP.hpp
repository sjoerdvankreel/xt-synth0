#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/SynthModel.hpp"

namespace Xts {

enum class EnvStage { Dly, A, Hld, D, S, R, End };

struct EnvOutput
{
  float lvl;
  bool staged;
  EnvStage stage;
};

struct EnvParams
{
  float dly, hld;
  float a, d, s, r;
};

class EnvDSP 
{
  int _stagePos = 0;
  float _level = 0.0f;
  EnvStage _stage = EnvStage::Dly;
  EnvStage _prevStage = EnvStage::Dly;

  void NextStage(EnvStage stage);
  void CycleStage(EnvModel const& env, EnvParams const& params);
  float Generate(float from, float to, float len, int slp) const;
  float Generate(EnvModel const& env, EnvParams const& params) const;

public:
  void Init(EnvModel const& env);
  void Release(EnvModel const& env);
  float Frames(EnvParams const& params);
  EnvParams Params(EnvModel const& env, float rate, int bpm) const;
  void Next(EnvModel const& env, float rate, int bpm, EnvOutput& output);
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP