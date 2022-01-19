#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct EnvParams;
enum class EnvStage { Dly, A, Hld, D, S, R, End };

class EnvDSP 
{
  int _stagePos = 0;
  float _level = 0.0f;
  EnvStage _stage = EnvStage::Dly;
public:
  EnvDSP() = default;
  EnvDSP(EnvDSP const&) = delete;
private:
  void NextStage(EnvStage stage);
  void CycleStage(EnvType type, EnvParams const& params);
  float Generate(float from, float to, float len, int slp) const;
  float Generate(EnvModel const& model, EnvParams const& params) const;
  EnvParams Params(EnvModel const& model, AudioInput const& input) const;
public:
  bool End() const { return _stage == EnvStage::End; }
  void Init(EnvModel const& model, AudioInput const& input);
  float Next(EnvModel const& model, AudioInput const& input);
  void Release(EnvModel const& model, AudioInput const& input);
  void Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(Generator<EnvDSP, EnvModel, float>);

} // namespace Xts
#endif // XTS_ENV_DSP_HPP