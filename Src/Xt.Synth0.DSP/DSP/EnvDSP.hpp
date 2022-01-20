#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct EnvParams;
enum class EnvStage { Dly, A, Hld, D, S, R, End };

class EnvDSP 
{
  float _level;
  int _stagePos;
  EnvStage _stage;
  EnvModel const* const _model;
  AudioInput const* const _input;
public:
  EnvDSP() = default;
  EnvDSP(EnvDSP const&) = delete;
  EnvDSP(EnvModel const* model, AudioInput const* input);
private:
  void NextStage(EnvStage stage);
  float Generate(EnvParams const& params) const;
  void CycleStage(EnvType type, EnvParams const& params);
  float Generate(float from, float to, float len, int slp) const;
  static EnvParams Params(EnvModel const& model, AudioInput const& input);
public:
  float Next();
  void Release();
  bool End() const { return _stage == EnvStage::End; }
  static void Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(FiniteGenerator<EnvDSP, EnvModel, float>);

} // namespace Xts
#endif // XTS_ENV_DSP_HPP