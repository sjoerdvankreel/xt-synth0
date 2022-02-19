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

class EnvDSP
{
  int _pos;
  EnvStage _stage;
  EnvParams _params;
  float _max, _value;
  EnvModel const* _model;
  double _slp, _lin, _log;
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, float bpm, float rate);
private:
  float Generate();
  void CycleStage(EnvType type);
  void NextStage(EnvStage stage);
  float Generate(float from, float to, SlopeType type);
  static EnvParams Params(EnvModel const& model, float bpm, float rate);
public:
  void Next();
  void Release();
  bool End() const { return _stage == EnvStage::End; }
  static void Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output);
  float Value() const { return (_model->on && _model->inv) ? 1.0f - _value : _value; }
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP