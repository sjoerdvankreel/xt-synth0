#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct EnvParams;
enum class EnvStage { Dly, A, Hld, D, S, R, End };

class EnvDSP:
public DSPBase<EnvModel, SourceInput, float>
{
  int _pos;
  float _level;
  EnvStage _stage;
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, SourceInput const* input);
private:
  void NextStage(EnvStage stage);
  float Generate(EnvParams const& params) const;
  void CycleStage(EnvType type, EnvParams const& params);
  float Generate(float from, float to, int len, int slp) const;
  static EnvParams Params(EnvModel const& model, SourceInput const& input);
public:
  void Next();
  void Release();
  bool End() const { return _stage == EnvStage::End; }
  static void Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output);
  float Value() const { return (_model->on && _model->inv) ? 1.0f - _value : _value; }
};
static_assert(StateSourceDSP<EnvDSP, EnvModel>);
static_assert(ReleaseableDSP<EnvDSP, EnvModel, SourceInput, float>);
static_assert(FiniteSourceDSP<EnvDSP, EnvModel, SourceInput, float>);

} // namespace Xts
#endif // XTS_ENV_DSP_HPP