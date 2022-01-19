#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "DSP.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

enum class EnvStage { Dly, A, Hld, D, S, R, End };

struct EnvOutput
{
  bool end;
  float lvl;
public:
  EnvOutput() = default;
  EnvOutput(EnvOutput const&) = delete;
};

class EnvDSP 
{
  int _stagePos = 0;
  float _level = 0.0f;
  EnvStage _stage = EnvStage::Dly;
private:
  void NextStage(EnvStage stage);
  void CycleStage(EnvType type, struct EnvParams const& params);
  float Generate(float from, float to, float len, int slp) const;
  float Generate(EnvModel const& model, struct EnvParams const& params) const;
  void Params(EnvModel const& model, AudioState const& state, struct EnvParams& params) const;
public:
  void Init(EnvModel const& model);
  void Release(EnvModel const& model);
  void Next(EnvModel const& model, AudioState const& state, EnvOutput& output);
  void Plot(EnvModel const& model, AudioState const& state, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP