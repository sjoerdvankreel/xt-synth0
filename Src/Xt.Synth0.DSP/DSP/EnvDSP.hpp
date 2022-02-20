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

class EnvDSP
{
  int _pos;
  float _max;
  EnvOutput _output;
  EnvParams _params;
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
  void Release();
  EnvOutput Next();
  EnvOutput Output() const;
  bool End() const { return _output.stage == EnvStage::End; }
  static void Plot(EnvModel const& model, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP