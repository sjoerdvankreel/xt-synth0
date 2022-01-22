#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct EnvParams;
enum class EnvStage { Dly, A, Hld, D, S, R, End };

class EnvDSP:
private GeneratorDSP<EnvModel>
{
  int _pos;
  float _level;
  EnvStage _stage;
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, AudioInput const* input);
private:
  void NextStage(EnvStage stage);
  float Generate(EnvParams const& params) const;
  void CycleStage(EnvType type, EnvParams const& params);
  float Generate(float from, float to, int len, int slp) const;
  static EnvParams Params(EnvModel const& model, AudioInput const& input);
public:
  float Next();
  void Release();
  bool End() const { return _stage == EnvStage::End; }
  static void Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output);
};
static_assert(FiniteDSP<EnvDSP, EnvModel>);
static_assert(StateSource<EnvDSP, EnvModel>);

} // namespace Xts
#endif // XTS_ENV_DSP_HPP