#ifndef XTS_DSP_SYNTH_ENV_DSP_HPP
#define XTS_DSP_SYNTH_ENV_DSP_HPP

#include <DSP/Synth/EnvSample.hpp>
#include <Model/Synth/EnvModel.hpp>

namespace Xts {

struct EnvParams
{
  float sustain;
  int holdSamples;
  int delaySamples;
  int decaySamples;
  int attackSamples;
  int releaseSamples;
};

struct EnvPlotState
{
  int hold;
  EnvModel const* model;
  struct PlotOutput* output;
  struct PlotInput const* input;
};

class EnvDSP
{
  int _pos;
  float _max;
  double _base;
  double _increment;
  EnvSample _output;
  EnvParams _params;
  EnvModel const* _model;
public:
  bool End() const;
  EnvSample Next();
  EnvSample Release();
  EnvSample Output() const;
  static void Plot(EnvPlotState* state);
public:
  EnvDSP() = default;
  EnvDSP(EnvModel const* model, float bpm, float rate);
private:
  float Generate();
  void CycleStage(EnvType type);
  void NextStage(EnvStage stage);
  float Generate(float from, float to, SlopeType type);
  static EnvParams Params(EnvModel const& model, float bpm, float rate);
};

inline bool
EnvDSP::End() const
{ return _output.stage == EnvStage::End; }

} // namespace Xts
#endif // XTS_DSP_SYNTH_ENV_DSP_HPP