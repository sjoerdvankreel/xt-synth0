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
  EnvelopeSample _output;
  EnvParams _params;
  EnvModel const* _model;
  double _slp, _lin, _log;
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, float bpm, float rate);
private:
  float Generate();
  void CycleStage(EnvType type);
  void NextStage(EnvelopeStage stage);
  float Generate(float from, float to, SlopeType type);
  static EnvParams Params(EnvModel const& model, float bpm, float rate);
public:
  EnvelopeSample Next();
  EnvelopeSample Release();
  EnvelopeSample Output() const;
  bool End() const { return _output.stage == EnvelopeStage::End; }
  static void Plot(EnvModel const& model, int hold, PlotInput const& input, PlotOutput& output);
};

} // namespace Xts
#endif // XTS_ENV_DSP_HPP