#ifndef XTS_ENV_DSP_HPP
#define XTS_ENV_DSP_HPP

#include "../Model/DSPModel.hpp"
#include "../Model/SynthModel.hpp"

namespace Xts {

struct EnvParams
{
  float s;
  int dly, a, hld, d, r;
public:
  EnvParams() = default;
  EnvParams(EnvParams const&) = default;
};

enum class EnvStage { Dly, A, Hld, D, S, R, End };

class EnvDSP:
public DSPBase<EnvModel, SourceInput, float>
{
  int _pos;
  float _max;
  EnvStage _stage;
  EnvParams _params;
  double _slp, _lin, _log;
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, SourceInput const* input);
private:
  float Generate();
  void CycleStage(EnvType type);
  void NextStage(EnvStage stage);
  float Generate(float from, float to, int len, SlopeType type);
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